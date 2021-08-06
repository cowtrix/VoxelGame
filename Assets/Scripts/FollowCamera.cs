using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class FollowCamera : ExtendedMonoBehaviour
{
	public Vector3 Offset;
	protected Camera CurrentCamera => CameraController.Instance.GetComponent<Camera>();

	private void Update()
	{
		transform.position = CurrentCamera.transform.position + Offset;
	}
}
