using UnityEngine;
using vSplines;

namespace Vehicles.AI
{
    public class VehiclePathTester : MonoBehaviour
    {
        public Transform Target;

        private void OnDrawGizmos()
        {
            var spline = new Spline();
            spline.Segments.Add(new SplineSegment // We add a new segment
            {
                FirstControlPoint = new SplineSegment.ControlPoint
                {
                    Position = new Vector3(-10, 0, -10),
                    Control = new Vector3(10, 0, 0),      // Determines the normal of the spline point
                },
                SecondControlPoint = new SplineSegment.ControlPoint
                {
                    Position = new Vector3(10, 0, 10),
                    Control = new Vector3(0, 0, -10),
                },
                Resolution = .25f,  // Determines how many points are calculated on the spline path
            });
            spline.Recalculate();
            spline.DrawGizmos(Color.white);

            var distance = 4.5f;
            var point = spline.GetDistancePointAlongSpline(distance);
            Gizmos.DrawCube(point, Vector3.one);
        }
    }
}
