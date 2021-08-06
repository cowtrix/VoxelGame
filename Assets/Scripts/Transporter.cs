using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour
{
    public Vector3 Rotation;
    public Transform Exit;
	public Vector3 Offset;

	public Vector3 ExitPosition => transform.localToWorldMatrix.MultiplyPoint3x4(transform.worldToLocalMatrix.MultiplyPoint3x4(Exit.position) + Offset);

	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<MovementController>();
		if (!player)
		{
			return;
		}
		player.transform.position = ExitPosition;
		//player. *= transform.rotation * Quaternion.Euler(Rotation);
	}

	private void OnDrawGizmosSelected()
	{
		if (!Exit)
		{
			return;
		}
		Gizmos.DrawCube(ExitPosition, Vector3.one * 6);
	}
}
