using UnityEngine;

public class RadialGravitySource : GravitySource
{
	public float Radius = 100;
	public float Strength = 100;
	public AnimationCurve Falloff;

	public override Vector3 GetGravityForce(Vector3 position)
	{
		var grav = position - transform.position;
		var dist = grav.sqrMagnitude / (Radius * Radius);
		var str = Falloff.Evaluate(dist) * Strength;
		return grav.normalized * str;
	}
}
