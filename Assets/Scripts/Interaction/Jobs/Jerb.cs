using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jerbs
{
    public abstract class Jerb : TrackedObject<Jerb>
    {
        public Sprite Icon;
        public string Name;
        public string Description;
        public string Reward;

		public Vector3 GetCurrentPosition() => transform.position;
    }
}