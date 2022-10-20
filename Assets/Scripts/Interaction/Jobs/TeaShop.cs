using Interaction;
using Interaction.Activities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Jerbs
{
    public class TeaShop : Jerb
    {
        public bool PressureValveOpen { get; private set; }
        public float Pressure { get; private set; }
        public float TimeAtMaxPressure { get; private set; }
        public float Temperature { get; private set; }
        public float WaterLevel { get; private set; }
        public float FuelAmount { get; private set; }

        [Header("Visuals")]
        public Label PressureGauge, TemperatureGauge;
        private TransformMover m_pressureGaugeMover, m_tempGaugeMover;
        public ToggleInteractable WaterTap, BurnerInlet;
        public ParticleSystem WaterTapFlow, DefaultFire, HotFire;
        public Door WaterValve, BurnerDoor;
        public Transform WaterLevelIndicator;
        public BasicMachine PressureValveSteam, PressureLimitReached;

        public VoxelColorTint[] FuelLogs;

        [Header("Simulation")]
        public float WaterFlowRate = 1;
        public float WaterTapRate = 1;
        public float PressureReleaseRate = 1;
        public float PressureReleaseTime = 3;
        public float PressureBuildUpSpeed = 1;
        public float BurnerOpenHeat = 1;
        public float BurnerClosedHeat = 1;
        public float BurnerCoolSpeed = 1;
        public float FuelBurnSpeed = 1;


        private void Start()
        {
            m_pressureGaugeMover = PressureGauge.GetComponent<TransformMover>();
            m_tempGaugeMover = TemperatureGauge.GetComponent<TransformMover>();
        }

        public void AddFuel()
        {
            FuelAmount = Mathf.Clamp(FuelAmount + 1, 0, FuelLogs.Length);
        }

        private void Update()
        {
            UpdateSimulation();
            UpdateVisuals();
        }

        void UpdateSimulation()
        {
            // Refill or empty the water tank
            WaterLevel = Mathf.Clamp01(WaterLevel + WaterValve.OpenAmount * WaterFlowRate * Time.deltaTime - (WaterTap.ToggleState ? WaterTapRate * Time.deltaTime : 0));
            // We're adding cool water so reduce by the open flow rate
            Temperature /= 1 + WaterValve.OpenAmount * WaterFlowRate * Time.deltaTime;
            if (PressureValveOpen)
            {
                WaterLevel -= PressureReleaseRate * Time.deltaTime;
                Temperature -= PressureReleaseRate * Time.deltaTime;
            }

            // Burn some fuel
            FuelAmount = Mathf.Clamp(FuelAmount - FuelBurnSpeed * Time.deltaTime, 0, FuelLogs.Length);
            if (FuelAmount > 0)
            {
                // If some fuel exists, increase temperature
                if (BurnerInlet.ToggleState)
                {
                    // If grate is open less hot
                    Temperature = Mathf.Clamp(Temperature + (BurnerOpenHeat * Time.deltaTime) / WaterLevel, 0, 1);
                    if(Pressure > WaterLevel * Temperature)
                    {
                        Pressure -= PressureBuildUpSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    // If grate is closed more hot
                    Temperature = Mathf.Clamp(Temperature + (BurnerClosedHeat * Time.deltaTime) / WaterLevel, 0, 1);
                    Pressure += PressureBuildUpSpeed * Time.deltaTime;
                }
                FuelAmount = Mathf.Max(0, FuelAmount - FuelBurnSpeed * Time.deltaTime);
                DefaultFire.Play();
            }
            else
            {
                // If we're out of fuel, cool down
                Temperature = Mathf.Clamp(Temperature - BurnerCoolSpeed * Time.deltaTime, 0, 1);
                DefaultFire.Stop();
            }
            Temperature = Mathf.Clamp(Temperature, 0, WaterLevel * 2);


            Pressure = Mathf.Max(Pressure, WaterLevel * Temperature);
            if (Pressure >= 1)
            {
                TimeAtMaxPressure += Time.deltaTime;
            }
            if (TimeAtMaxPressure > 0)
            {
                if (TimeAtMaxPressure > PressureReleaseTime)
                {
                    PressureValveOpen = true;
                }
            }
            if (Pressure < .2f)
            {
                TimeAtMaxPressure -= Time.deltaTime;
                PressureValveOpen = false;
            }
        }

        void UpdateVisuals()
        {
            WaterLevelIndicator.localScale = new Vector3(1, WaterLevel, 1);
            PressureValveSteam.SetPower(PressureValveOpen);
            if (WaterTap.ToggleState && WaterLevel > 0 && !WaterTapFlow.isPlaying)
            {
                WaterTapFlow.Play();
            }
            else if ((!WaterTap.ToggleState || WaterLevel <= 0) && WaterTapFlow.isPlaying)
            {
                WaterTapFlow.Stop();
            }
            if (!BurnerInlet.ToggleState && !BurnerDoor.ToggleState)
            {
                if (!HotFire.isPlaying)
                {
                    HotFire.Play();
                }
            }
            else
            {
                if (HotFire.isPlaying)
                {
                    HotFire.Stop();
                }
            }
            TemperatureGauge.PlainText = $"Temperature: {Temperature:P0}";
            m_tempGaugeMover.OpenAmount = Temperature;
            PressureGauge.PlainText = $"Pressure: {Pressure:P0}";
            m_pressureGaugeMover.OpenAmount = Pressure;
            PressureLimitReached.SetPower(Pressure >= 1);
            for (int i = 0; i < FuelLogs.Length; i++)
            {
                var log = FuelLogs[i];
                log.gameObject.SetActive(i < FuelAmount);
            }
        }
    }
}