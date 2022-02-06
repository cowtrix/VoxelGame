using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generation
{
	[CreateAssetMenu]
	public class ObjectSet : ScriptableObject
	{
		[System.Serializable]
		public class Entry
		{
			public Object Object;
			[Range(1, 100)]
			public float Probability = 1;
		}

		public List<Entry> Data = new List<Entry>();

		public T GetWeightedRandom<T>() where T : class
		{
			var sum = Data.Sum(x => x.Probability);
			var roll = Random.value * sum;
			var accum = 0f;
			foreach (var obj in Data)
			{
				if (accum + obj.Probability > roll)
				{
					return obj.Object as T;
				}
				accum += obj.Probability;
			}
			return Data.Last().Object as T;
		}
	}
}