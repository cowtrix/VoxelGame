using System;
using System.Collections;
using UnityEngine;

namespace Common
{
	public class SlowUpdateManager : Singleton<SlowUpdateManager>
	{
		private void Start()
		{
			StartCoroutine(ThinkAll());
		}

		private IEnumerator ThinkAll()
		{
			while (true)
			{
				var t = Time.time;
				foreach (var i in SlowUpdater.Instances)
				{
					if (!i)
					{
						continue;
					}
					try
					{
						var dt = t - i.LastUpdateTime;
						if(dt < i.ThinkSpeed)
						{
							continue;
						}
						i.LastUpdateTime = t;
						i.Think(dt);
					}
					catch(Exception e)
					{
						Debug.LogException(e, i);
					}
				}
				yield return null;
			}
		}
	}
}