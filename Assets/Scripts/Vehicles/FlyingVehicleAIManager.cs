using Common;
using Generation.Spawning;
using Splines;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Voxul;
using Voxul.Utilities;

namespace Vehicles.AI
{
    public class FlyingVehicleAIManager : SlowUpdater
    {
        public List<Collider> Colliders { get; private set; }
        public Vector3 CurrentTargetPosition { get; private set; }
        public float CurrentTargetDistance { get; private set; }
        public Spline CurrentPath { get; private set; }
        public eState CurrentState { get; private set; }
        public bool BlockedByObstacle { get; private set; }

        public UnityEvent OnReachDestination;
        public LayerMask CollisionCheckMask;
        public Bounds CollisionBounds;
        public float Speed = 10;
        public float TurnSpeed = 1;


        public enum eState
        {
            WANDER_TRANSIENT, // The vehicle will wander to a distant vehicle bin
        }

        private void Start()
        {
            Colliders = GetComponentsInChildren<Collider>().ToList();
        }

        protected override int Tick(float dt)
        {
            if (CurrentState == eState.WANDER_TRANSIENT && CurrentPath == null)
            {
                var targetBin = GetDistantBin();
                if (!VehiclePathManager.Instance.GetPath(transform.position, (transform.position - targetBin.transform.position).normalized * Speed, targetBin.transform.position, out var newPath))
                {
                    return 2;
                }
                CurrentPath = newPath;
                CurrentTargetPosition = CurrentPath.Start;
                OnReachDestination.RemoveListener(OnReachDestination_WanderTransient);
                OnReachDestination.AddListener(OnReachDestination_WanderTransient);
                return 2;
            }
            var colliders = Physics.OverlapBox(transform.localToWorldMatrix.MultiplyPoint3x4(CollisionBounds.center + Vector3.forward * CollisionBounds.size.z), CollisionBounds.extents, transform.rotation, CollisionCheckMask);
            BlockedByObstacle = colliders.Any(c => !Colliders.Contains(c));
            return 0;
        }

        private void Update()
        {
            if (CurrentPath == null)
            {
                Tick(0);
                return;
            }
            if (CurrentPath != null && Vector3.Distance(transform.position, CurrentTargetPosition) < .1f)
            {
                CurrentTargetDistance += 1;
                CurrentTargetPosition = CurrentPath.GetDistancePointAlongSpline(CurrentTargetDistance);
            }
            if (!BlockedByObstacle)
            {
                transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPosition, Speed * Time.deltaTime);
            }
            transform.RotateTowardsPosition(CurrentPath.GetDistancePointAlongSpline(CurrentTargetDistance + 1), TurnSpeed * Time.deltaTime, Quaternion.identity);
            if (CurrentTargetDistance > CurrentPath.Length)
            {
                OnReachDestination.Invoke();
                CurrentPath = null;
            }
        }

        public ObjectBin GetDistantBin()
        {
            return ObjectBin.Instances.OrderBy(b => Vector3.Distance(b.transform.position, transform.position)).Last();
        }

        public void OnReachDestination_WanderTransient()
        {
            gameObject.SafeDestroy();
        }

        private void OnDrawGizmosSelected()
        {
            if (CurrentPath != null)
            {
                CurrentPath.DrawGizmos(Color.white);
            }
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Util.WithAlpha(Color.white, .2f);
            Gizmos.DrawWireCube(CollisionBounds.center, CollisionBounds.size);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(CollisionBounds.center + Vector3.forward * CollisionBounds.size.z, CollisionBounds.size);
        }
    }
}
