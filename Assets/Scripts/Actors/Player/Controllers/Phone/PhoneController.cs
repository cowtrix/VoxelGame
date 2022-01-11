using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Voxul;

[Serializable]
public class PhoneAppEvent : UnityEvent<Actor, PhoneApp> { }

public class PhoneController : ExtendedMonoBehaviour
{
	public PlayerInput Input;
	private PlayerActor m_actor;
	public Camera Camera;
	public InputSystemUIInputModule InputModule;
	public Vector3 OpenPosition, ClosedPosition;
	public float MoveSpeed = 1;

	public UnityEvent OnHome;
	public PhoneAppEvent OnAppFocused;

	public bool IsOpen { get; private set; }

	public RectTransform Cursor, CursorClick, Container;
	public float CursorSensitivity = 1;

	private InputAction m_togglePhoneAction, m_lookAction, m_clickAction;
	private CameraController CameraController => CameraController.Instance;

	public PhoneApp CurrentApp { get; private set; }
	public List<PhoneApp> Apps;
	public PhoneAppLaunchButton LaunchButtonPrefab;
	public Transform LaunchContainer;
	public Text TimeText, CreditsText;

	private void Start()
	{
		m_togglePhoneAction = Input.actions.Single(a => a.name == "TogglePhone");
		m_lookAction = Input.actions.Single(a => a.name == "Look");
		m_clickAction = Input.actions.Single(a => a.name == "Fire");
		m_togglePhoneAction.performed += OnTogglePhone;
		m_actor = Input.GetComponent<PlayerActor>();

		foreach (var app in Apps)
		{
			var launchButton = Instantiate(LaunchButtonPrefab.gameObject).GetComponent<PhoneAppLaunchButton>();
			launchButton.transform.SetParent(LaunchContainer);
			launchButton.transform.localPosition = Vector3.zero;
			launchButton.transform.localRotation = Quaternion.identity;
			launchButton.transform.localScale = Vector3.one;
			launchButton.SetApp(this, app);

			app.gameObject.SetActive(false);
		}

		OnHome?.Invoke();
	}

	public void OpenApp(PhoneApp app)
	{
		CurrentApp?.OnClose(this);
		CurrentApp = app;
		CurrentApp?.OnOpen(this);
		OnAppFocused?.Invoke(m_actor, CurrentApp);
	}

	public void Home()
	{
		CurrentApp?.OnClose(this);
		CurrentApp = null;
		OnHome?.Invoke();
	}

	private void OnTogglePhone(InputAction.CallbackContext obj)
	{
		IsOpen = !IsOpen;
		CameraController.LockCameraLook = IsOpen;
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

		CursorClick.gameObject.SetActive(m_clickAction.IsPressed());

		m_actor.State.TryGetValue<int>(nameof(ICreditedActor.Credits), out var credits);
		CreditsText.text = $"¢{credits}";
		TimeText.text = GameManager.Instance.GetTimeString();
	}
}
