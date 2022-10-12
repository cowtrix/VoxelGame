using Common;
using UnityEngine.Events;
using Voxul;

namespace Interaction.Activities
{
	public class BasicMachine : SlowUpdater
	{
		public bool IsPoweredOn;
		private bool? m_lastPower;

		public void SetPower(bool power) => IsPoweredOn = power;

		protected override int TickOnThread(float dt)
		{
			if(m_lastPower.HasValue && m_lastPower == IsPoweredOn)
            {
				return 0;
            }
			m_lastPower = IsPoweredOn;
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