using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
	public class SlowUpdateManager : Singleton<SlowUpdateManager>
	{
		public Func<SlowUpdater, float> InstanceSorter;
		public int FrameBudget = 100;

		private List<SlowUpdater> m_instances = new List<SlowUpdater>();

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
					m_instances = instances.OrderBy(InstanceSorter).AsParallel().ToList();
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
				foreach (var i in m_instances)
				{
					if (!i)
					{
						continue;
					}
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
				yield return null;
			}
		}
	}
}