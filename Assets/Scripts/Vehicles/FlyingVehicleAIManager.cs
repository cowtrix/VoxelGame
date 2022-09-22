using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Vehicles.AI
{
    public class FlyingVehicleAIManager : ExtendedMonoBehaviour 
    {
        public Transform Target;
        public float Speed = 10;
        public float LaneSnapDistance = 100;

        private void OnDrawGizmos()
        {
            var path = VehiclePathManager.Instance.GetPath(transform.position, transform.forward * Speed, Target.position, LaneSnapDistance);
            path.DrawGizmos(Color.green);

            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
        }
    }
}
