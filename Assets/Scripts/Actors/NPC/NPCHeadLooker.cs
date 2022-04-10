using Common;
using Interaction.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Actors.NPC
{
	public class NPCHeadLooker : SlowUpdater, ILookAdapter
	{
		public float MaxLookDistance = 10;
		[Range(0, 90)]
		public float LookAngle = 60;
		public float LookSpeed = 1;

		public Vector3 LookRotation;
		public RotationLimits LookRotationLimits;

		public NPCObservable CurrentTarget { get; protected set; }

		private void Update()
		{
			// Hacky but works
			var lastRot = transform.rotation;
			transform.LookAt(CurrentTarget?.transform, transform.parent.up);
			transform.rotation = Quaternion.Lerp(lastRot, transform.rotation, LookSpeed * Time.deltaTime);
		}

		private void OnDrawGizmosSelected()
		{
			GizmoExtensions.DrawCone(transform.position, Quaternion.Euler(LookRotation) * transform.parent.forward, Mathf.Deg2Rad * LookAngle, MaxLookDistance, CurrentTarget ? Color.white : Color.gray);
			if (CurrentTarget)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, CurrentTarget.transform.position);
			}
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
	}
}