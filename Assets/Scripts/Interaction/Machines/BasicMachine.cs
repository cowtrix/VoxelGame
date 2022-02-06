using UnityEngine.Events;
using Voxul;

namespace Interaction.Activities
{
	public class BasicMachine : ExtendedMonoBehaviour
	{
		public bool IsPoweredOn;

		private void Update()
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