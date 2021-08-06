using UnityEngine;
using Voxul;

[ExecuteAlways]
public class LookAtPlayer : ExtendedMonoBehaviour
{
	protected Camera CurrentCamera => CameraController.Instance.GetComponent<Camera>();

	private void Update()
	{
		transform.LookAt(CurrentCamera.transform);
	}
}