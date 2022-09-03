using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class LookTester : ExtendedMonoBehaviour
{
	public Transform Target;
	[Range(0, 90)]
	public float MaxAngle = 1;

	private void Update()
	{
		//transform.RotateTowardsPosition(Target.position, Time.deltaTime * MaxAngle);
	}

    private void OnDrawGizmos()
    {
		Gizmos.DrawLine(transform.position, Target.position);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
    }
}
  
