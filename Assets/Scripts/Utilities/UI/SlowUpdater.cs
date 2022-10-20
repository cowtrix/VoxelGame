using UnityEngine;

namespace Common
{
    public abstract class SlowUpdater : TrackedObject<SlowUpdater>
    {
        public class SlowUpdateState
        {
            public uint OnThreadUpdateCount { get; set; }
            public uint OffThreadUpdateCount { get; set; }
            public float LastOnThreadUpdateTime { get; set; }
            public float LastOffThreadUpdateTime { get; set; }
            public bool RequiresUpdate { get; set; }
            public Vector3 LastPosition { get; set; }
        }
        public SlowUpdateState SlowUpdateInfo { get; private set; } = new SlowUpdateState();

        public float ThinkSpeed = 1;
        public int Priority = 0;

        public virtual float GetThinkSpeed() => ThinkSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();
            ThinkOffThread(0);
            ThinkOnThread(0);
        }

        private void Update()
        {
            SlowUpdateInfo.LastPosition = transform.position;
        }

        public void ThinkOffThread(float dt)
        {
            SlowUpdateInfo.OffThreadUpdateCount++;
            TickOffThread(dt);
        }

        public int ThinkOnThread(float dt)
        {
            //Profiler.BeginSample($"Think: {GetType().Name}");
            SlowUpdateInfo.OnThreadUpdateCount++;
            var cost = TickOnThread(dt);
            //Profiler.EndSample();
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
        protected virtual void TickOffThread(float dt) { }
    }
}