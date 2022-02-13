using UnityEngine;
using Voxul;

[ExecuteAlways]
public class StarOrbiter : ExtendedMonoBehaviour 
{
	public GameManager GameManager => GameManager.Instance;

	private void Update()
	{
		transform.localRotation = Quaternion.Euler(0, GameManager.NormalizedTimeOfDay * 360, 0);
	}
}
