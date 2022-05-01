using UnityEngine.Profiling;
using Voxul;

namespace Common
{
	public abstract class SlowUpdater : TrackedObject<SlowUpdater>
	{
		public float LastUpdateTime { get; set; }
		public float ThinkSpeed = 1;
		public int Priority = 0;

		public int Think(float dt)
		{
			Profiler.BeginSample($"Think: {GetType().Name}");
			var cost = Tick(dt);
			Profiler.EndSample();
			return cost;
		}

		protected abstract int Tick(float dt);
	}
}