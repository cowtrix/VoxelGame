using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

namespace Actors.Dritch
{
    public class DritchlekCrimeManager : Singleton<DritchlekCrimeManager>
    {
        public Light Star, Alarm;
        public GameObject AlarmCube;
        public float WantedLevelReductionSpeed = .1f;
        public float WantedLevel;

        private void Update()
        {
            WantedLevel = Mathf.Clamp(WantedLevel - Time.deltaTime * WantedLevelReductionSpeed, 0, 5);
            if (WantedLevel <= 0)
            {
                AlarmCube.gameObject.SetActive(false);
                Alarm.enabled = false;
                Star.enabled = true;
            }
            else
            {
                AlarmCube.gameObject.SetActive(true);
                Alarm.enabled = true;
                Star.enabled = false;
            }
        }

        public void OnPerceiveEvent(ActorEventData perceptionEvent)
        {
            if (!perceptionEvent.IsIllegal)
            {
                return;
            }
            WantedLevel++;
        }
    }
}
