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

		private List<SlowUpdater> m_instances = new List<SlowUpdater>();
		private object m_lock = new object();

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
					bool isSorting = true;
					var t = new Task(() =>
					{
						var newList = instances.OrderBy(InstanceSorter).ToList();
						lock (m_lock)
						{
							m_instances = newList;
						}
						isSorting = false;
					});
					t.Start();
					while (isSorting)
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
					for (int iter = 0; iter < m_instances.Count; iter++)
					{
						var i = m_instances[iter];
						try
						{
							var dt = t - i.LastUpdateTime;
							if (dt < i.ThinkSpeed)
							{
								continue;
							}
							i.LastUpdateTime = t;
							budgetCounter += i.Think(dt);
						}
						catch (Exception e)
						{
							Debug.LogException(e, i);
						}
						if (budgetCounter > FrameBudget)
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