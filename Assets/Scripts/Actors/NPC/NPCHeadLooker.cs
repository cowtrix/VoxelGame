using Common;
using Interaction.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Actors.NPC
{
    public class NPCHeadLooker : SlowUpdater, ILookAdapter
    {
        public float MaxLookDistance = 10;
        [Range(0, 90)]
        public float LookAngle = 60;
        public float LookSpeed = 1;

        public Vector3 LookRotation;
        public Vector3 Forward = Vector3.forward;
        public RotationLimits LookRotationLimits;

        public NPCObservable CurrentTarget { get; protected set; }

        public Vector3 CurrentLookPosition
        {
            get
            {
                if (CurrentTarget)
                {
                    return CurrentTarget.transform.position;
                }
                else
                {
                    return transform.position + transform.parent.localToWorldMatrix.MultiplyVector(Forward);
                }
            }
        }

        private void Update()
        {
            transform.RotateTowardsPosition(CurrentLookPosition, Time.deltaTime * LookSpeed, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            GizmoExtensions.DrawCone(transform.position, Quaternion.Euler(LookRotation) * transform.parent.forward, Mathf.Deg2Rad * LookAngle, MaxLookDistance, CurrentTarget ? Color.white : Color.gray);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, CurrentLookPosition);
        }

        protected override int Tick(float dt)
        {
            var closestScaledDistance = float.MaxValue;
            NPCObservable closestObservable = null;
            var allObservables = NPCObservable.Instances.ToList();
            foreach (var lookTarget in allObservables)
            {
                if (!lookTarget.enabled)
                {
                    continue;
                }
                var distance = Vector3.Distance(transform.position, lookTarget.transform.position);
                if (distance > lookTarget.AttentionDistance || distance > MaxLookDistance ||
                    !Mathfx.PointIsInCone(lookTarget.transform.position, transform.position, Quaternion.Euler(LookRotation) * transform.parent.forward, Mathf.Deg2Rad * LookAngle))
                {
                    continue;
                }
                var scaledDistance = distance * lookTarget.AttentionPriority;
                if (scaledDistance < closestScaledDistance)
                {
                    closestScaledDistance = scaledDistance;
                    closestObservable = lookTarget;
                }
            }
            CurrentTarget = closestObservable;
            return Mathf.CeilToInt(allObservables.Count / 3);
        }

        public bool CanSee(Vector3 worldPosition)
        {
            // TODO
            return true;
        }
    }
}