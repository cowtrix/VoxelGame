using Common;
using UnityEngine.Events;
using Voxul;

namespace Interaction.Activities
{
	public class BasicMachine : SlowUpdater
	{
		public bool IsPoweredOn;

		public void SetPower(bool power) => IsPoweredOn = power;

		protected override int Tick(float dt)
		{
			if (IsPoweredOn)
			{
				PowerOn.Invoke();
			}
			else
			{
				PowerOff.Invoke();
			}
			return 1;
		}

		public UnityEvent PowerOn;
		public UnityEvent PowerOff;
	}
}