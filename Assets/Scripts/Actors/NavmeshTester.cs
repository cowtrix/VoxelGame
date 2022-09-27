using UnityEngine;
using UnityEngine.AI;
using Voxul;

namespace Actors
{
    public class NavmeshTester : ExtendedMonoBehaviour
    {
        public Transform Target;
        public NavMeshPath Path { get; private set; }

        private void OnDrawGizmos()
        {
            if(Path == null)
            {
                Path = new NavMeshPath();
            }
            Gizmos.DrawCube(transform.position, Vector3.one * .3f);
            Gizmos.DrawCube(Target.position, Vector3.one * .3f);
            NavMesh.CalculatePath(transform.position, Target.position, 1, Path);
            Vector3 lastPoint = default;
            for (int i = 0; i < Path.corners.Length; i++)
            {
                var p = Path.corners[i];
                if(i > 0)
                {
                    Gizmos.DrawLine(lastPoint, p);
                }
                lastPoint = p;
            }
        }
    }
}