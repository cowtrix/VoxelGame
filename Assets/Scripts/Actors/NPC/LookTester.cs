using Common;
using UnityEngine;
using Voxul;

public class LookTester : ExtendedMonoBehaviour
{
	public Transform Target, Test;
	[Range(0, 90)]
	public float Angle = 90;

	private void OnDrawGizmos()
	{
		var lookVector = Target.position - transform.position;

		Color c = Color.white;
		if (Mathfx.PointIsInCone(Test.position, transform.position, lookVector.normalized, Mathf.Deg2Rad * Angle))
			c = Color.green;

		GizmoExtensions.DrawCone(transform.position, lookVector.normalized, Mathf.Deg2Rad * Angle, lookVector.magnitude, c);
	}
}
  
