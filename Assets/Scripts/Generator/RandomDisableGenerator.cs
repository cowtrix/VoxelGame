using System;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
	public class RandomDisableGenerator : ExtendedMonoBehaviour, IGenerationCallback
	{
		public Guid LastGenerationID { get ; set; }

		[Range(0,1)]
		public float ChanceOfDestruction = .5f;

		public void Generate(ObjectGenerator objectGenerator)
		{
			gameObject.SetActive(UnityEngine.Random.value > ChanceOfDestruction);
		}
	}
}