using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		GizmoExtensions.DrawWireCube(transform.position, new Vector3(1, 2, 1) * .5f, transform.rotation, Color.green);
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
	}
}
