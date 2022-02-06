using Common;
using UnityEngine;

public class LevelGravitySource : GravitySource
{
	private Vector3 m_targetGravityDirection;
	public float GravityTransitionSpeed = 1;
	public Vector3 GravityDirection = Vector3.down;
	public float GravityStrength = 1000;
	[Header("Axis Gravity")]
	public Bounds AxisGravityBounds = new Bounds(Vector3.zero, Vector3.one);
	[Header("Radial Gravity")]
	public float Radius = 100;

	public AnimationCurve Falloff;

	private void Start()
	{
		m_targetGravityDirection = GravityDirection;
	}

	public override Vector3 GetGravityForce(Vector3 position)
	{
		var localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(position);
		if (AxisGravityBounds.Contains(localPos))
		{
			return GravityDirection * GravityStrength;
		}
		var grav = transform.position - position;
		var dist = grav.sqrMagnitude / (Radius * Radius);
		var str = Falloff.Evaluate(dist) * GravityStrength;
		return grav.normalized * str * 0;
	}

	public override void SetGravity(Vector3 dir)
	{
		if (!dir.IsOnAxis())
		{
			return;
		}
		m_targetGravityDirection = dir;
	}

	private void Update()
	{
		/*var playerPos = CameraController.Instance.transform.position;
		playerPos = transform.worldToLocalMatrix.MultiplyPoint3x4(playerPos);
		if (!AxisGravityBounds.Contains(playerPos))
		{
			m_targetGravityDirection = -(playerPos - transform.position).normalized.ClosestAxisNormal();
		}
		m_targetGravityDirection = m_targetGravityDirection.ClosestAxisNormal();
		GravityDirection = Vector3.RotateTowards(GravityDirection, m_targetGravityDirection,
			GravityTransitionSpeed * Time.deltaTime, 1);*/
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + m_targetGravityDirection);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(AxisGravityBounds.center, AxisGravityBounds.size);
		Gizmos.DrawWireSphere(AxisGravityBounds.center, Radius);
	}
}
