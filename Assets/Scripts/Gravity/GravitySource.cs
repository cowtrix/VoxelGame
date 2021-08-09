using Common;
using UnityEngine;


public abstract class GravitySource : TrackedObject<GravitySource>
{
	public bool Exclusive = true;
	public abstract Vector3 GetGravityForce(Vector3 position);
	public abstract void SetGravity(Vector3 vector3);
}
