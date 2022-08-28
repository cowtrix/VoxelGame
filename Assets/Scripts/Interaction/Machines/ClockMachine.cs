using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction
{
    public class ClockMachine : ExtendedMonoBehaviour
    {
        protected GameManager GameManager => GameManager.Instance;
        public Transform HourHand, MinuteHand;

        private void Update()
        {
            var t = GameManager.CurrentTime;
            HourHand.localRotation= Quaternion.Euler(0,0, 360 * (t.Hour / (GameDateTime.HoursInDay / 2f)));
            MinuteHand.localRotation = Quaternion.Euler(0, 0, 360 * (t.Minute / 60f));
            // Secondhand moves a little disconc
            //SecondHand.localRotation = Quaternion.Euler(0, 0, 360 * (t.Second / 60f));
        }
    }
}

