using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJobs
{
    public class GameJob : TrackedObject<GameJob>
    {
        public Sprite Icon;
        public string Name;
        public string Description;
        public string Reward;

		public Vector3 GetCurrentPosition() => transform.position;
    }
}