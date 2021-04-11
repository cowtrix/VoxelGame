using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTester : MonoBehaviour
{
	public sbyte Layer = 0;
	private void OnDrawGizmos()
	{
		
		var thisPoint = VoxelCoordinate.FromVector3(transform.position, Layer);
		Gizmos.DrawCube(thisPoint.ToVector3(), Vector3.one * .1f);

		GizmoExtensions.Label(thisPoint.ToVector3(), $"Manhatten Distance: {thisPoint.ManhattenDistance(default)}");

		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward.ClosestAxisNormal());

	}
}
