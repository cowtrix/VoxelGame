using UnityEngine.Profiling;
using Voxul;

namespace Common
{
	public abstract class SlowUpdater : TrackedObject<SlowUpdater>
	{
		public uint UpdateCount { get; private set; }
		public float LastUpdateTime { get; set; }
		public float ThinkSpeed = 1;
		public int Priority = 0;

		public int Think(float dt)
		{
			Profiler.BeginSample($"Think: {GetType().Name}");
			UpdateCount++;
			var cost = Tick(dt);
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

        public virtual float GetThinkSpeed() => ThinkSpeed;
		protected abstract int Tick(float dt);
	}
}