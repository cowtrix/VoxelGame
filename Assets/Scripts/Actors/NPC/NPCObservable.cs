using Common;
using UnityEngine;

public class NPCObservable : TrackedObject<NPCObservable>
{
	public float AttentionDistance = 10;
	public int AttentionPriority;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		GizmoExtensions.DrawSphere(Vector3.zero, Quaternion.identity, AttentionDistance, Color.gray);
	}
}
