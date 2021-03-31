using UnityEngine;


public abstract class GravitySource : MonoBehaviour
{
	public abstract Vector3 GetGravityForce(Vector3 position);
}
