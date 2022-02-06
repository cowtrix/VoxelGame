using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
	public interface IGenerationCallback
	{
		Guid LastGenerationID { get; set; }
		void Generate(ObjectGenerator objectGenerator);
	}

	public class ObjectGenerator : ExtendedMonoBehaviour
	{
		public Guid LastGenerationID { get; private set; }
		public List<IGenerationCallback> Children => new List<IGenerationCallback>(gameObject.GetComponentsByInterfaceInChildren<IGenerationCallback>());
		public int Seed;

		private void OnValidate()
		{
			if (Seed == 0)
			{
				Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
		}

		[ContextMenu("Generate")]
		public void Generate()
		{
			UnityEngine.Random.InitState(Seed);
			LastGenerationID = Guid.NewGuid();
			IEnumerable<IGenerationCallback> dirtyChildren;
			do
			{
				dirtyChildren = Children.Where(c => c.LastGenerationID != LastGenerationID).ToList();
				foreach (var c in dirtyChildren)
				{
					if(c == null || c.Equals(null))
					{
						continue;
					}
					c.Generate(this);
					c.LastGenerationID = LastGenerationID;
				}
			}
			while (dirtyChildren.Any());

		}
	}
}