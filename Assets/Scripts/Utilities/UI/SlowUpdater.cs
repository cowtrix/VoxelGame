using UnityEngine.Profiling;
using Voxul;

namespace Common
{
	public abstract class SlowUpdater : TrackedObject<SlowUpdater>
	{
		public uint OnThreadUpdateCount { get; private set; }
		public uint OffThreadUpdateCount { get; private set; }
		public float LastOnThreadUpdateTime { get; set; }
		public float LastOffThreadUpdateTime { get; set; }

		public float ThinkSpeed = 1;
		public int Priority = 0;

		public virtual float GetThinkSpeed() => ThinkSpeed;

		public int ThinkOffThread(float dt)
        {
			OffThreadUpdateCount++;
			var cost = TickOffThread(dt);
			return cost;
		}

		public int Think(float dt)
		{
			Profiler.BeginSample($"Think: {GetType().Name}");
			OnThreadUpdateCount++;
			var cost = TickOnThread(dt);
			Profiler.EndSample();
			return cost;
		}

        protected override void OnDestroy()
        {
            if (SlowUpdateManager.HasInstance())
            {
				SlowUpdateManager.Instance.DeRegister(this);
            }
			base.OnDestroy();
        }

		protected virtual int TickOnThread(float dt) => 0;
		protected virtual int TickOffThread(float dt) => 0;
	}
}