using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class Star : ExtendedMonoBehaviour
{
	public float Offset = 2000;
	protected Camera CurrentCamera => CameraController.Instance.GetComponent<Camera>();

	private void Update()
	{
		transform.position = CurrentCamera.transform.position + Vector3.one * Offset;
		transform.LookAt(CurrentCamera.transform);
	}
}
