using Actors;
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Voxul.Utilities;

public interface ICameraControllerProxy
{
	public Quaternion? LookDirectionOverride { get; }
	public Vector3? LookPositionOverride { get; }

	void Look(Actor actor, Vector2 lastDelta);
}

public class CameraController : Singleton<CameraController>, ILookAdapter
{
	public Actor Actor;
	public bool LockCameraLook { get; set; }
	public bool LockCursor { get; set; } = true;
	public bool UIEnabled { get; set; }
	public Camera Camera { get; private set; }
	public InputSystemUIInputModule InputModule { get; private set; }

	[Header("Camera")]
	public float LookSensitivity = 1;
	public Vector2 LookAngle, LastDelta;
	public Vector3 LookOffset = new Vector3(0, .5f, 0);
	private InputAction m_look;

	[Header("Proxy")]
	public float ProxyChaseSpeed = 1;
	public ICameraControllerProxy Proxy { get; set; }

	public PlayerInput Input => transform.parent.GetComponent<PlayerInput>();

	private void Start()
	{
		m_look = Input.actions.Single(a => a.name == "Look");
		Camera = GetComponent<Camera>();
		InputModule = GetComponent<InputSystemUIInputModule>();
	}

	private void Update()
	{
		InputModule.enabled = UIEnabled;
		Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
		LastDelta = m_look.ReadValue<Vector2>() * LookSensitivity;
		if (LockCameraLook)
		{
			return;
		}
		LookAngle += LastDelta * Time.deltaTime;
		while (LookAngle.x < -180) LookAngle.x += 360;
		while (LookAngle.x > 180) LookAngle.x -= 360;
		LookAngle.y = Mathf.Clamp(LookAngle.y, -89, 89);

		if (Proxy != null)
		{
			Proxy.Look(Actor, LastDelta);
			if (Proxy.LookDirectionOverride.HasValue)
			{
				transform.rotation = Proxy.LookDirectionOverride.Value;
			}
			else
			{
				transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
			}
			if (Proxy.LookPositionOverride.HasValue)
			{
				transform.position = Vector3.Lerp(transform.position, Proxy.LookPositionOverride.Value, Time.deltaTime * ProxyChaseSpeed);
			}
			else
			{
				transform.localPosition = LookOffset;
			}
		}
		else
		{
			transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
			transform.localPosition = LookOffset;
		}
	}

	public void LookAt(Vector3 forward)
	{
		// Change to parent local
		forward = transform.parent.worldToLocalMatrix.MultiplyVector(forward);
		LookAngle.x = Vector2.Angle(forward.Flatten(), transform.forward.Flatten());
		//LookAngle.y = Vector2.Angle(forward.yz(), transform.forward.yz());
	}
}
