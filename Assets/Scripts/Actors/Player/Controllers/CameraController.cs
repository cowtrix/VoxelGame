using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
	public bool LockCameraLook { get; set; }
	public bool LockCursor { get; set; } = true;

	[Header("Camera")]
	public float LookSensitivity = 1;
	public Vector2 LookAngle, LastDelta;
	private InputAction m_look;

	private void Start()
	{
		m_look = transform.parent.GetComponent<PlayerInput>().actions.Single(a => a.name == "Look");
	}

	private void Update()
	{
		Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
		LastDelta = m_look.ReadValue<Vector2>() * LookSensitivity;
		if(LockCameraLook)
		{
			return;
		}
		LookAngle += LastDelta * Time.deltaTime;
		while (LookAngle.x < -180) LookAngle.x += 360;
		while (LookAngle.x > 180) LookAngle.x -= 360;
		LookAngle.y = Mathf.Clamp(LookAngle.y, -89, 89);
		transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
	}
}
