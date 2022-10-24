using UnityEngine;
using vSplines;

namespace Vehicles.AI
{
    public class VehiclePathTester : MonoBehaviour
    {
        public Transform Target;

        private void OnDrawGizmos()
        {
            var path = VehiclePathManager.Instance.GetPath(transform.position, (transform.position - Target.position).normalized, Target.position, out var spline);
            spline.DrawGizmos(path ? Color.green : Color.red);
        }
    }
}
