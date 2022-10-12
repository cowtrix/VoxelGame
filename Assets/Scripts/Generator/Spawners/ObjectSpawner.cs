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
        public int InstanceCountLimit = 10;
        private float m_spawnTimer;

        protected override int TickOnThread(float dt)
        {
            m_spawnTimer -= dt;
            for (int i = TrackedInstances.Count - 1; i >= 0; i--)
            {
                if (!TrackedInstances[i])
                {
                    TrackedInstances.RemoveAt(i);
                }
            }
            if(TrackedInstances.Count >= InstanceCountLimit)
            {
                return 0;
            }
            if(m_spawnTimer <= 0)
            {
                var prefab = Prefabs.Random();
                var newInstance = Instantiate(prefab);
                newInstance.transform.SetParent(transform);
                newInstance.transform.Reset(true);
                var rb = newInstance.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.position = transform.position;
                }
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