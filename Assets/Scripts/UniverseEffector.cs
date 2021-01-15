using Common;
using UnityEngine;

public class UniverseEffector : TrackedObject<UniverseEffector>
{
	public Vector3 Size;
	public float Scale = 1;

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Size);
	}
}
