using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Activities.Dotc
{
    public class DotcAncient : DotcGameEntity
    {
        public List<DotcGameCreep> Creeps { get; private set; } = new List<DotcGameCreep>();
        public int CreepAmmo = 0;
        public int MaxCreepAmmo = 5;

        public float CreepTime = 1;
        public DotcGameCreep CreepPrefab;

        private float m_creepTimer;

        protected override void Start()
        {
            CreepPrefab.gameObject.SetActive(false);
            base.Start();
        }

        public void SpawnCreep(Transform lane)
        {
            if(CreepAmmo <= 0)
            {
                Debug.Log("Can't spawn, no ammo");
                return;
            }
            Debug.Log("Spawned new creep");
            CreepAmmo--;
            var newCreep = Instantiate(CreepPrefab.gameObject).GetComponent<DotcGameCreep>();
            Creeps.Add(newCreep);
            newCreep.Faction = Faction;
            newCreep.transform.position = transform.position;
            newCreep.transform.SetParent(Game.transform);
            newCreep.gameObject.SetActive(true);
            newCreep.CurrentPath = Game.GetPathTo(Faction, lane).ToQueue();
            newCreep.WakeUp();
        }

        private void Update()
        {
            if(CreepAmmo < MaxCreepAmmo)
            {
                m_creepTimer -= Time.deltaTime;
                if (m_creepTimer < 0)
                {
                    CreepAmmo++;
                    m_creepTimer = CreepTime;
                }
            }
        }
    }

}