using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class NPCHeadLooker : ExtendedMonoBehaviour
{
	public float MaxLookDistance = 10;
	[Range(0, 90)]
	public float LookAngle = 60;
	public float LookSpeed = 1;

	public Vector3 LookRotation;

	public NPCObservable CurrentTarget { get; protected set; }

	private void Start()
	{
		StartCoroutine(Think());
	}

	IEnumerator Think()
	{
		while (true)
		{
			var closestScaledDistance = float.MaxValue;
			NPCObservable closestObservable = null;
			foreach (var lookTarget in NPCObservable.Instances)
			{
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
				yield return null;
			}
			CurrentTarget = closestObservable;
			yield return null;
		}
	}

	private void Update()
	{
		Vector3 dir;
		var extraLookRotation = Quaternion.Euler(LookRotation);
		var forward = extraLookRotation * transform.parent.forward;
		if (CurrentTarget)
		{
			dir = (CurrentTarget.transform.position - transform.position).normalized;
		}
		else
		{
			dir = forward;
		}
		Debug.DrawLine(transform.position, transform.position + forward, Color.blue);
		Debug.DrawLine(transform.position, transform.position + dir, Color.blue);

		var inverseRot = Quaternion.Inverse(extraLookRotation);
		transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, inverseRot * dir, LookSpeed * Time.deltaTime, 0), transform.parent.up);
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
}
