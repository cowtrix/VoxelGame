using UnityEngine.Events;
using Voxul;

namespace Interaction.Activities
{
	public class BasicMachine : SlowUpdater
	{
		public bool IsPoweredOn;

		public void SetPower(bool power) => IsPoweredOn = power;

		protected override void Tick(float dt)
		{
			if (IsPoweredOn)
			{
				PowerOn.Invoke();
			}
			else
			{
				PowerOff.Invoke();
			}
		}

		public UnityEvent PowerOn;
		public UnityEvent PowerOff;
	}
}