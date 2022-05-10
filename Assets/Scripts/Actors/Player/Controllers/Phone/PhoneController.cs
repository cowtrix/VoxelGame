using Actors;
using Actors.NPC.Player;
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

namespace Phone
{
	[Serializable]
	public class PhoneAppEvent : UnityEvent<Actor, PhoneApp> { }

	public class PhoneController : ExtendedMonoBehaviour
	{
		public PlayerInput Input;
		public Camera Camera;
		public InputSystemUIInputModule InputModule;
		public Vector3 OpenPosition, ClosedPosition;
		public float MoveSpeed = 1;
		public UnityEvent OnHome;
		public PhoneAppEvent OnAppFocused;

		public bool IsOpen { get; private set; }
		public PlayerActor Actor { get; private set; }

		public RectTransform Cursor, CursorClick, Container;
		public float CursorSensitivity = 1;

		private InputAction m_togglePhoneAction, m_lookAction, m_clickAction;
		private CameraController CameraController => CameraController.Instance;
		public PhoneNotificationManager NotificationManager;
		public PhoneApp CurrentApp { get; private set; }
		public List<PhoneApp> Apps;
		public PhoneAppLaunchButton LaunchButtonPrefab;
		public Transform LaunchContainer;
		public Text TimeText, CreditsText;
		public AudioClip PingAudio;

		private void Start()
		{
			m_togglePhoneAction = Input.actions.Single(a => a.name == "TogglePhone");
			m_lookAction = Input.actions.Single(a => a.name == "Look");
			m_clickAction = Input.actions.Single(a => a.name == "Fire");
			m_clickAction.performed += ClickActionPerformed;
			m_togglePhoneAction.performed += OnTogglePhone;
			Actor = Input.GetComponent<PlayerActor>();

			foreach (var app in Apps)
			{
				var launchButton = Instantiate(LaunchButtonPrefab.gameObject).GetComponent<PhoneAppLaunchButton>();
				launchButton.transform.SetParent(LaunchContainer);
				launchButton.transform.localPosition = Vector3.zero;
				launchButton.transform.localRotation = Quaternion.identity;
				launchButton.transform.localScale = Vector3.one;
				launchButton.SetApp(this, app);

				app.Initialise(this);

				app.gameObject.SetActive(false);
			}

			OnHome?.Invoke();
		}

		private void ClickActionPerformed(InputAction.CallbackContext obj)
		{
			Ping();
		}

		public void OpenApp(PhoneApp app)
		{
			CurrentApp?.OnClose(this);
			CurrentApp = app;
			CurrentApp?.OnOpen(this);
			OnAppFocused?.Invoke(Actor, CurrentApp);
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

		public T GetApp<T>() where T : PhoneApp
		{
			return Apps.FirstOrDefault(a => a is T) as T;
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

			Actor.State.TryGetValue<int>(eStateKey.Credits, out var credits);
			CreditsText.text = $"¢{credits}";
			TimeText.text = GameManager.Instance.CurrentTime.GetTimeString();
		}

		public void Ping()
		{
			if (!IsOpen)
			{
				return;
			}
			AudioSource.PlayClipAtPoint(PingAudio, transform.position, .5f);
		}
	}
}