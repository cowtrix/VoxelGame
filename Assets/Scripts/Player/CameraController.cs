using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public PlayerInput Input;

	public Vector2 ZoomLimits = new Vector2(5, 10);
	public float LookSensitivity = 1;
	public Vector2 LookAngle;

	private float m_currentZoom;
	private InputAction m_look, m_zoom;

	private void Start()
	{
		m_look = Input.actions.Single(a => a.name == "Look");
		m_zoom = Input.actions.Single(a => a.name == "Zoom");
	}

	private void Update()
	{
		var lookDelta = m_look.ReadValue<Vector2>() * LookSensitivity * Time.deltaTime;
		m_currentZoom = Mathf.Clamp(m_currentZoom + m_zoom.ReadValue<float>(), ZoomLimits.x, ZoomLimits.y);

		LookAngle += lookDelta;
		transform.localPosition = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0) * Vector3.back * m_currentZoom;
		transform.LookAt(Input.transform);
	}
}
