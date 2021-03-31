using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class CameraController : Singleton<CameraController>
{
	[Header("Camera")]
	public float LookSensitivity = 1;
	public Vector2 LookAngle;
	
	[Header("References")]
	public PlayerInput Input;

	private InputAction m_look;

	private void Start()
	{
		m_look = Input.actions.Single(a => a.name == "Look");
	}

	private void Update()
	{
		var lookDelta = m_look.ReadValue<Vector2>() * LookSensitivity * Time.deltaTime;
		LookAngle += lookDelta;
		while (LookAngle.x < -180) LookAngle.x += 360;
		while (LookAngle.x > 180) LookAngle.x -= 360;
		LookAngle.y = Mathf.Clamp(LookAngle.y, -89, 89);

		transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
	}
}
