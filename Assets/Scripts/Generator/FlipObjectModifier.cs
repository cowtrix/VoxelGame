using System;
using Voxul;

namespace Generation
{
	public class FlipObjectModifier : ExtendedMonoBehaviour, IGenerationCallback
	{
		public bool FlipX, FlipY, FlipZ;
		public Guid LastGenerationID { get; set; }

		public void Generate(ObjectGenerator objectGenerator)
		{
			bool flip() => UnityEngine.Random.value > .5f;
			transform.localScale = new UnityEngine.Vector3(
					FlipX ? (flip() ? -1 : 1) * transform.localScale.x : transform.localScale.x,
					FlipY ? (flip() ? -1 : 1) * transform.localScale.y : transform.localScale.y,
					FlipZ ? (flip() ? -1 : 1) * transform.localScale.z : transform.localScale.z
				);
		}
	}
}