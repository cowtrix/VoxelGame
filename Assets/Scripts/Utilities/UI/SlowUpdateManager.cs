using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Voxul.Utilities;

namespace Common
{
    public class SlowUpdateManager : Singleton<SlowUpdateManager>
	{
		public Func<SlowUpdater, float> InstanceSorter;
		public int FrameBudget = 100;
		public int MaxCyclePopulation = 1000;

		private List<SlowUpdater> m_instances = new List<SlowUpdater>();
		private object m_lock = new object();
		private static bool m_isSorting;

		private void Start()
		{
			StartCoroutine(SortUpdaters());
			StartCoroutine(ThinkAll());
		}

		private IEnumerator SortUpdaters()
		{
			while (true)
			{
				var instances = SlowUpdater.Instances;
				if (InstanceSorter != null)
				{
					m_isSorting = true;
					var data = instances.Select(i => (InstanceSorter(i), i))
						.ToList();
					var t = new Task(() =>
					{
						try
						{
							var newList = data
								.OrderBy(d => d.Item2.Priority)
								.ThenBy(d => d.Item1)
								.Reverse()
								.Select(d => d.Item2)
								.ToList();
							lock (m_lock)
							{
								m_instances = newList;
							}
						}
						catch(Exception e)
						{
							Debug.LogException(e);
						}
						finally
						{
							m_isSorting = false;
						}
					});
					t.Start();
					while (m_isSorting)
					{
						yield return null;
					}
					yield return new WaitForSeconds(1);
				}
			}
		}

		private IEnumerator ThinkAll()
		{
			var firstIteration = true;
			while (true)
			{
				var t = Time.time;
				var budgetCounter = 0;
				lock (m_lock)
				{
					for (int i = m_instances.Count - 1; i >= 0; i--)
					{
						var instance = m_instances[i];
						if (!instance)
						{
							m_instances.RemoveAt(i);
							continue;
						}
						if (!instance.enabled || !instance.gameObject.activeInHierarchy)
						{
							continue;
						}
						try
						{
							var dt = t - instance.LastUpdateTime;
							if (dt < instance.ThinkSpeed)
							{
								continue;
							}
							instance.LastUpdateTime = t;
							budgetCounter += instance.Think(dt);
						}
						catch (Exception e)
						{
							Debug.LogException(e, instance);
						}
						if (firstIteration)
						{
							continue;
						}
						if (budgetCounter > FrameBudget)
						{
							budgetCounter = 0;
							yield return null;
						}
						if(i < m_instances.Count - MaxCyclePopulation)
						{
							break;
						}
					}
					firstIteration = false;
				}
				yield return null;
			}
		}
	}
}