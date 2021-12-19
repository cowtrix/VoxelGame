using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Voxul;

public class PhoneController : ExtendedMonoBehaviour
{
	public PlayerInput Input;
	public Camera Camera;
	public InputSystemUIInputModule InputModule;
	public Vector3 OpenPosition, ClosedPosition;
	public float MoveSpeed = 1;
	public bool IsOpen { get; private set; }

	public RectTransform Cursor, Container;
	public float CursorSensitivity = 1;

	private InputAction m_togglePhoneAction, m_lookAction;
	private CameraController CameraController => CameraController.Instance;

	private void Start()
	{
		m_togglePhoneAction = Input.actions.Single(a => a.name == "TogglePhone");
		m_lookAction = Input.actions.Single(a => a.name == "Look");
		m_togglePhoneAction.performed += OnTogglePhone;
	}

	private void OnTogglePhone(InputAction.CallbackContext obj)
	{
		IsOpen = !IsOpen;
		CameraController.LockCameraLook = IsOpen;
		//CameraController.LockCursor = !IsOpen;
	}

	private void Update()
	{
		var dt = Time.deltaTime;
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, IsOpen ? OpenPosition : ClosedPosition, dt * MoveSpeed);
		if (!IsOpen)
		{
			InputModule.ExplicitMousePosition = null;
			return;
		}

		var lookDelta = m_lookAction.ReadValue<Vector2>() * CursorSensitivity;
		var canvasSize = Container.sizeDelta;
		Cursor.anchoredPosition = new Vector2(
			Mathf.Clamp(Cursor.anchoredPosition.x + lookDelta.x, -canvasSize.x / 2, canvasSize.x / 2),
			Mathf.Clamp(Cursor.anchoredPosition.y + lookDelta.y, -canvasSize.y / 2, canvasSize.y / 2)
			);

		var screenPoint = Camera.WorldToScreenPoint(Cursor.position).xy();
		InputModule.ExplicitMousePosition = screenPoint;

		var debugPoint = Camera.ScreenToWorldPoint(screenPoint.xy0(5));
		DebugHelper.DrawPoint(debugPoint, .1f, Color.cyan, 0);

		/*var screenPoint = Camera.WorldToViewportPoint(Cursor.position).xy();
		InputModule.ExplicitMousePosition = new Vector2(Screen.width * screenPoint.x, Screen.height * screenPoint.y);

		var debugPoint = Camera.ViewportToWorldPoint(screenPoint.xy0(5));
		DebugHelper.DrawPoint(debugPoint, .1f, Color.cyan, 0);*/
	}
}
