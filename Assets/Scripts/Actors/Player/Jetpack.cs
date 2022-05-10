using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.NPC.Player
{
    public class Jetpack : MonoBehaviour
    {
        public bool Active { get; set; }
        public bool ThrustersFiring { get; set; }

        public float ThrusterStrength = 10;
        public PlayerActor Player;
        public Transform GravityIndicator;
        public Transform JoyStick;

        public Vector3 ActivePosition;
        public Vector3 InactivePosition;
        public float ActiveMoveSpeed = 1;
        public float FuelEfficiency = -.05f;
        public Gauge FuelGauge;
        public AudioSource ThrusterSound;
        public Vector2 ThrusterVolumes;
        public float ThrusterVolumeSensitivity = 1;
        public float ThrusterVolumeSpeed = 1;
        private float m_currentThrusterVolume;
        public float JoystickRotateStrength = 1;
        public float JoystickRotateSpeed = 1;

        

        private void Update()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Active ? ActivePosition : InactivePosition, Time.deltaTime * ActiveMoveSpeed);

            var grav = Player.MovementController.CurrentGravity;
            GravityIndicator.LookAt(transform.position + grav);

            var move = Player.MovementController.MoveDirection;
            JoyStick.localRotation = Quaternion.Lerp(JoyStick.localRotation, Quaternion.Euler(move.y * JoystickRotateStrength, 0, -move.x * JoystickRotateStrength), JoystickRotateSpeed * Time.deltaTime);

            if(Player.State.TryGetValueNormalized(eStateKey.Fuel, out var fuel))
            {
                FuelGauge.Value = fuel;
            }

            m_currentThrusterVolume = (Active && fuel > 0 && ThrustersFiring) ? Mathf.Clamp01(ThrusterVolumes.x + (ThrusterVolumeSensitivity - ThrusterVolumes.x) / (ThrusterVolumes.y - ThrusterVolumes.x)) : 0;
            ThrusterSound.volume = Mathf.MoveTowards(ThrusterSound.volume, m_currentThrusterVolume, Time.deltaTime * ThrusterVolumeSpeed);
        }
    }
}
