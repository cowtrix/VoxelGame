using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Voxul;


namespace Actors
{
    public class NPCSimpleMovementController : ExtendedMonoBehaviour, IMovementController
    {
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public NavMeshAgent Navmesh => GetComponent<NavMeshAgent>();
        public Actor Actor => GetComponent<Actor>();
        public Vector3 CurrentGravity => GravityManager.Instance.GetGravityForce(transform.position);
        public Vector3 MoveDirection { get; set; }
        public bool IsGrounded => Navmesh.isOnNavMesh;

        public float MinVelocity = .01f;
        public float AnimationExpressiveness = 1;
        public NavMeshQueryFilter NavmeshFilter;
        private NavMeshPath m_currentPath;
        private Vector3 m_lastPosition;

        private void Update()
        {
            if (Actor.Animator)
            {
                var localVelocity = transform.worldToLocalMatrix.MultiplyVector(transform.position - m_lastPosition) * AnimationExpressiveness;
                if(localVelocity.magnitude < MinVelocity)
                {
                    localVelocity = Vector3.zero;
                }
                Debug.DrawLine(transform.position, transform.position + transform.localToWorldMatrix.MultiplyVector(localVelocity), Color.cyan);
                Actor.Animator.SetFloat("VelocityX", localVelocity.z);
                Actor.Animator.SetFloat("VelocityY", localVelocity.y);
                Actor.Animator.SetFloat("VelocityZ", localVelocity.x);
            }
            m_lastPosition = transform.position;
        }

        public void MoveToPosition(Vector3 worldPos)
        {
            m_currentPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(transform.position, worldPos, NavmeshFilter, m_currentPath))
            {
                m_currentPath = null;
                return;
            }
            Navmesh.SetPath(m_currentPath);
        }

        public void Jump()
        {
        }

        public void MoveInput(Vector2 dir)
        {
        }

        private void OnDrawGizmosSelected()
        {
            if(Navmesh.path != null)
            {
                for (int i = 0; i < Navmesh.path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(Navmesh.path.corners[i], Navmesh.path.corners[i + 1]);
                }
            }
        }
    }
}