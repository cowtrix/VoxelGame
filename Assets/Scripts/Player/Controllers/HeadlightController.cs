using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Light))]
public class HeadlightController : MonoBehaviour
{
	public PlayerInput Input;
	private PlayerState PlayerState => Input.GetComponent<PlayerState>();

	private bool HeadlightOn
	{
		get => PlayerState.CurrentState.HeadlightOn;
		set => PlayerState.CurrentState.HeadlightOn = value;
	}

	private InputAction m_headlightToggle;
	private Light m_light;

	private void Awake()
	{
		m_headlightToggle = Input.actions.Single(a => a.name == "ToggleHeadlight");
		m_light = GetComponent<Light>();
	}

	private void Update()
	{
		if (m_headlightToggle.triggered)
		{
			HeadlightOn = !HeadlightOn;
		}
		m_light.enabled = HeadlightOn;
	}
}
