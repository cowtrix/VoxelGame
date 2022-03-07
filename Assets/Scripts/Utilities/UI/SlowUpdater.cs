using Voxul;

namespace Common
{

	public abstract class SlowUpdater : TrackedObject<SlowUpdater>
	{
		public float LastUpdateTime { get; set; }
		public float ThinkSpeed = 1;

		public void Think(float dt)
		{
			Tick(dt);
		}

		protected abstract void Tick(float dt);
	}
}