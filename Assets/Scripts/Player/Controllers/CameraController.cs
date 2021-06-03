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
	public Vector2 LookAngle, LastDelta;
	public bool LockView { get; set; }
	private InputAction m_look;

	private void Start()
	{
		m_look = transform.parent.GetComponent<PlayerInput>().actions.Single(a => a.name == "Look");
	}

	private void Update()
	{
		LastDelta = m_look.ReadValue<Vector2>() * LookSensitivity;
		if(LockView)
		{
			return;
		}
		LookAngle += LastDelta * Time.deltaTime;
		while (LookAngle.x < -180) LookAngle.x += 360;
		while (LookAngle.x > 180) LookAngle.x -= 360;
		LookAngle.y = Mathf.Clamp(LookAngle.y, -89, 89);
		transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
	}

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
}
