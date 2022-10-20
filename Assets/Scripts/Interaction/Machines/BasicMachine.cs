using Common;
using UnityEngine.Events;
using Voxul;

namespace Interaction.Activities
{
    public class BasicMachine : SlowUpdater
    {
        public bool IsPoweredOn { get; private set; }
        private bool m_isDirty;

        public void SetPower(bool power)
        {
            if(power == IsPoweredOn)
            {
                return;
            }
            IsPoweredOn = power;
            m_isDirty = true;
        }

        protected override int TickOnThread(float dt)
        {
            if (m_isDirty)
            {
                if (IsPoweredOn)
                {
                    PowerOn.Invoke();
                }
                else
                {
                    PowerOff.Invoke();
                }
                m_isDirty = false;
                return 10;
            }
            return 0;
        }

        public UnityEvent PowerOn;
        public UnityEvent PowerOff;
    }
}