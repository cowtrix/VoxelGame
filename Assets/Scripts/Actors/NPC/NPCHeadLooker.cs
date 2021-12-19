using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class NPCHeadLooker : ExtendedMonoBehaviour
{
	public Vector2 SpinLimits = new Vector2(-45, 45);
	public Vector2 YawLimits = new Vector2(-15, 20);
	public Vector2 TiltLimits = new Vector2(-10, 10);

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
		var forward = transform.parent.forward;
		if (CurrentTarget)
		{
			dir = Quaternion.Euler(AdditionalRotation) * (CurrentTarget.transform.position - transform.position).normalized;
		}
		else
		{
			dir = forward;
		}
		Debug.DrawLine(transform.position, transform.position + forward, Color.blue);
		Debug.DrawLine(transform.position, transform.position + dir, Color.blue);
		transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, dir, LookSpeed * Time.deltaTime, 0), transform.parent.up);
	}

	private void OnDrawGizmosSelected()
	{
		if (CurrentTarget)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, CurrentTarget.transform.position);
		}
	}
}
