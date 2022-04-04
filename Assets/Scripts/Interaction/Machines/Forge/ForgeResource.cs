using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Interaction.Activities
{
	public class ForgeResource : SlowUpdater
	{
		[Range(0,1)]
		public float TransitionAmount = 0;
		public List<ConveyorBelt> Path { get; set; }
		public ConveyorBelt CurrentLocation { get; set; }
		public ConveyorBelt TransitionLocation { get; set; }
		public ForgeConveyorManager Manager;
		private VoxelColorTint m_tint;

		[ColorUsage(false, true)]
		public Color TransitionStart, TransitionEnd;
		public Vector3 TransitionStartScale, TransitionEndScale;

		private void Start()
		{
			m_tint = GetComponent<VoxelColorTint>();
		}

		private void Update()
		{
			m_tint.Color = Color.Lerp(TransitionStart, TransitionEnd, TransitionAmount);
			m_tint.Invalidate();

			transform.localScale = Vector3.Lerp(TransitionStartScale, TransitionEndScale, TransitionAmount);
		}

		protected override int Tick(float dt)
		{
			if(Path == null)
			{
				return 0;
			}
			if(Path.Count == 0)
			{
				return 0;
			}
			var next = Path.Last();
			if(CurrentLocation == next)
			{
				Path.RemoveAt(Path.Count - 1);
				return 0;
			}
			if(CurrentLocation == TransitionLocation)
			{
				TransitionLocation = null;
			}
			var doubleNext = Path.ElementAtOrDefault(Path.Count - 2);
			if(!TransitionLocation && CurrentLocation != next && next.Receive(this, doubleNext))
			{
				TransitionLocation = next;
				Path.RemoveAt(Path.Count - 1);
				return 1;
			}
			return 0;
		}
	}
}