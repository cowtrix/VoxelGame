using Common;
using Generation.Spawning;
using vSplines;
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
        public Vector3 CurrentTargetLookPosition { get; private set; }
        public float CurrentTargetDistance { get; private set; }
        public float CurrentSpeed { get; private set; }
        public Spline CurrentPath { get; private set; }
        public Vector3? CurrentDestination { get; private set; }
        public Vector3 LastPosition { get; private set; }
        public bool RequiresPath { get; private set; }
        public eState CurrentState { get; private set; }
        public bool BlockedByObstacle { get; private set; }
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();

        public UnityEvent OnReachDestination;
        public LayerMask CollisionCheckMask;
        public Bounds CollisionBounds;
        public float MaxSpeed = 10;
        public float Thrust = 1;
        public float TurnSpeed = 30;


        public enum eState
        {
            WANDER_TRANSIENT, // The vehicle will wander to a distant vehicle bin
        }

        private void Start()
        {
            Colliders = GetComponentsInChildren<Collider>().ToList();
        }

        protected override int TickOnThread(float dt)
        {
            if (CurrentState == eState.WANDER_TRANSIENT && !CurrentDestination.HasValue)
            {
                CurrentDestination = GetDistantBin().transform.position;
            }

            var colliders = Physics.OverlapBox(CurrentTargetLookPosition, CollisionBounds.extents, transform.rotation, CollisionCheckMask);
            BlockedByObstacle = colliders.Any(c => !Colliders.Contains(c));
            return 1;
        }

        protected override void TickOffThread(float dt)
        {
            if (CurrentState == eState.WANDER_TRANSIENT && CurrentDestination.HasValue && CurrentPath == null)
            {
                if (!VehiclePathManager.Instance.GetPath(LastPosition, (LastPosition - CurrentDestination.Value).normalized * MaxSpeed, CurrentDestination.Value, out var newPath))
                {
                    return;
                }
                CurrentPath = newPath;
                CurrentTargetPosition = CurrentPath.Start;
                OnReachDestination.RemoveListener(OnReachDestination_WanderTransient);
                OnReachDestination.AddListener(OnReachDestination_WanderTransient);
            }
        }

        private void Update()
        {
            LastPosition = transform.position;
            if (CurrentPath != null && Vector3.Distance(transform.position, CurrentTargetPosition) < .1f)
            {
                CurrentTargetDistance += 1;
                CurrentTargetPosition = CurrentPath.GetDistancePointAlongSpline(CurrentTargetDistance);
                CurrentTargetLookPosition = CurrentPath.GetDistancePointAlongSpline(CurrentTargetDistance + CollisionBounds.size.z);
            }
            if (BlockedByObstacle)
            {
                CurrentSpeed = Mathf.Clamp(CurrentSpeed - Thrust * Time.deltaTime, 0, MaxSpeed);
            }
            else
            {
                CurrentSpeed = Mathf.Clamp(CurrentSpeed + Thrust * Time.deltaTime, 0, MaxSpeed);
            }
            if(CurrentPath != null)
            {
                Rigidbody.position = Vector3.MoveTowards(Rigidbody.position, CurrentTargetPosition, CurrentSpeed * Time.deltaTime);
                Rigidbody.RotateTowardsPosition(CurrentTargetLookPosition, TurnSpeed * Time.deltaTime, Quaternion.identity);
            }
            if (CurrentPath != null && CurrentTargetDistance > CurrentPath.Length)
            {
                OnReachDestination.Invoke();
                CurrentPath = null;
            }
        }

        public ObjectBin GetDistantBin()
        {
            return ObjectBin.Instances
                .Where(b => b.BinType == ObjectBin.eBinType.VEHICLE)
                .OrderBy(b => Vector3.Distance(b.transform.position, transform.position)).Last();
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
