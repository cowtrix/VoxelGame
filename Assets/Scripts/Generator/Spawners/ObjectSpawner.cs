using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation.Spawning
{
    public class ObjectSpawner : SlowUpdater
    {
        public List<GameObject> TrackedInstances { get; private set; } = new List<GameObject>();

        public List<GameObject> Prefabs;
        public Vector2 SpawnTime = new Vector2(10, 10);
        public bool RandomiseOnSpawn = true;
        private float m_spawnTimer;

        protected override int Tick(float dt)
        {
            m_spawnTimer -= dt;
            if(m_spawnTimer <= 0)
            {
                var prefab = Prefabs.Random();
                var newInstance = Instantiate(prefab);
                newInstance.transform.SetParent(transform);
                newInstance.transform.Reset();
                TrackedInstances.Add(newInstance);
                if (RandomiseOnSpawn)
                {
                    var generators = newInstance.GetComponentsInChildren<DynamicGeneratorNode>();
                    foreach(var creator in generators)
                    {
                        creator.RandomizeSeed();
                        creator.Generate();
                    }
                }
                m_spawnTimer = Random.Range(SpawnTime.x, SpawnTime.y);
                return 2;
            }
            return 0;
        }
    }
}