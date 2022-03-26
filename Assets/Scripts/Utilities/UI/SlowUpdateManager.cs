using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
							var newList = data.OrderBy(d => d.Item1)
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
			while (true)
			{
				var t = Time.time;
				var budgetCounter = 0;
				lock (m_lock)
				{
					for (int i = 0; i < m_instances.Count; i++)
					{
						var instance = m_instances[i];
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
						if (budgetCounter > FrameBudget)
						{
							yield return null;
						}
						if(i > MaxCyclePopulation)
						{
							break;
						}
					}
				}
				yield return null;
			}
		}
	}
}