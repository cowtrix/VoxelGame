using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using UnityEngine;
using Voxul.Utilities;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


namespace Common
{
    public class SlowUpdaterDistanceSorter : IComparer<SlowUpdater>
    {
        public Vector3 RootPosition { get; set; }
        public int Compare(SlowUpdater x, SlowUpdater y)
        {
            var distX = Vector3.Distance(x.SlowUpdateInfo.LastPosition, RootPosition);
            var distY = Vector3.Distance(y.SlowUpdateInfo.LastPosition, RootPosition);
            if (distX > distY)
                return 1;
            else if (distX == distY)
                return 0;
            else
                return -1;
        }
    }

    public class BucketCollection<T>
    {
        public int Count
        {
            get
            {
                var sum = 0;
                foreach(var bucket in Buckets)
                {
                    sum += bucket.Value.Count;
                }
                return sum;
            }
        }
        public Dictionary<int, List<T>> Buckets = new Dictionary<int, List<T>>();
        private IComparer<T> m_sorter;

        public BucketCollection(IComparer<T> sorter)
        {
            m_sorter = sorter;
        }

        public void TryAdd(int bucketID, T value)
        {
            if (!Buckets.TryGetValue(bucketID, out List<T> bucket))
            {
                bucket = new List<T>();
                Buckets.Add(bucketID, bucket);
            }
            if (bucket.Contains(value))
            {
                return;
            }
            bucket.Add(value);
        }

        public void Sort()
        {
            foreach (var bucket in Buckets)
            {
                bucket.Value.Sort(m_sorter);
            }
        }

        public void Remove(int bucketID, T value)
        {
            if (!Buckets.TryGetValue(bucketID, out List<T> bucket))
            {
                return;
            }
            bucket.Remove(value);
        }
    }

    public class SlowUpdateManager : Singleton<SlowUpdateManager>
    {
        public int CurrentPopulation { get; private set; }

        public Func<Vector3> RootPositionFunc;
        public int FrameBudget = 100;

        private BucketCollection<SlowUpdater> m_buckets;
        private SlowUpdaterDistanceSorter m_sorter;
        private float m_time;

        private void Start()
        {
            m_sorter = new SlowUpdaterDistanceSorter();
            m_sorter.RootPosition = RootPositionFunc();
            m_buckets = new BucketCollection<SlowUpdater>(m_sorter);

            StartCoroutine(SortUpdaters());
            StartCoroutine(ThinkOnThread());
            new Task(ThinkOffThread).Start();
        }

        private IEnumerator SortUpdaters()
        {
            while (true)
            {
                var instances = SlowUpdater.Instances;
                m_sorter.RootPosition = RootPositionFunc();
                var t = new Task(() =>
                {
                    try
                    {
                        foreach(var instance in instances)
                        {
                            m_buckets.TryAdd(instance.Priority, instance);
                        }
                        m_buckets.Sort();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
                t.Start();
                yield return new WaitForSeconds(10);

            }
        }

        public void DeRegister(SlowUpdater slowUpdater)
        {
            m_buckets.Remove(slowUpdater.Priority, slowUpdater);
        }

        private IEnumerator ThinkOnThread()
        {
            var keyList = new List<int>();
            while (true)
            {
                m_time = Time.time;
                var budgetCounter = 0;
                CurrentPopulation = m_buckets.Count;

                keyList.Clear();
                keyList.AddRange(m_buckets.Buckets.Keys);
                keyList.Sort();

                for (var i = 0; i < keyList.Count - 1; i++)
                {
                    var bucketID = keyList[i];
                    var bucket = m_buckets.Buckets[bucketID];
                    for(var j = 0; j < bucket.Count; j++)
                    {
                        var instance = bucket[j];
                        try
                        {
                            if (!instance.SlowUpdateInfo.RequiresUpdate)
                            {
                                continue;
                            }
                            instance.SlowUpdateInfo.RequiresUpdate = false;
                            instance.SlowUpdateInfo.LastOnThreadUpdateTime = m_time;
                            if (instance)
                            {
                                var dt = m_time - instance.SlowUpdateInfo.LastOffThreadUpdateTime;
                                budgetCounter += instance.ThinkOnThread(dt);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e, instance);
                        }
                    }
                    if (budgetCounter > FrameBudget)
                    {
                        budgetCounter = 0;
                        yield return null;
                    }
                }
                yield return null;
            }
        }

        private void ThinkOffThread()
        {
            var keyList = new List<int>();
            while (true)
            {
                keyList.Clear();
                keyList.AddRange(m_buckets.Buckets.Keys);
                keyList.Sort();

                for (var i = 0; i < keyList.Count - 1; i++)
                {
                    var bucketID = keyList[i];
                    var bucket = m_buckets.Buckets[bucketID];
                    for (var j = 0; j < bucket.Count; j++)
                    {
                        var instance = bucket[j];
                        try
                        {
                            var dt = m_time - instance.SlowUpdateInfo.LastOffThreadUpdateTime;
                            if (dt < instance.GetThinkSpeed())
                            {
                                continue;
                            }
                            instance.SlowUpdateInfo.RequiresUpdate = true;
                            instance.SlowUpdateInfo.LastOffThreadUpdateTime = m_time;
                            instance.ThinkOffThread(dt);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e, instance);
                        }
                    }
                }
            }
        }
    }
}