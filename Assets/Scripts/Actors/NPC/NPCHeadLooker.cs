using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class NPCHeadLooker : ExtendedMonoBehaviour
{
	public float LookSpeed = 1;
	public float LookRotationLimit;
	public Vector3 AdditionalRotation;

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
				if (distance > lookTarget.AttentionDistance)
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
		var forward = Quaternion.Euler(AdditionalRotation) * (transform.position + transform.parent.forward).normalized;
		if (CurrentTarget)
		{
			dir = Quaternion.Euler(AdditionalRotation) * (CurrentTarget.transform.position - transform.position).normalized;
			var angle = Vector3.Angle(forward, dir);
			if (angle < LookRotationLimit)
			{
				dir = -forward;
			}
		}
		else
		{
			dir = forward;
		}
		transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, dir, LookSpeed * Time.deltaTime, 0), transform.parent.up);
	}
}
