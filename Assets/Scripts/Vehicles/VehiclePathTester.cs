using UnityEngine;

namespace Vehicles.AI
{
    public class VehiclePathTester : MonoBehaviour
    {
        public Transform Target;

        private void OnDrawGizmos()
        {
            var validPath = VehiclePathManager.Instance.GetPath(transform.position, (transform.position - Target.position).normalized, Target.position, out var path);
            if (path != null)
            {
                path.DrawGizmos(validPath ? Color.white : Color.red);
            }
        }
    }
}
