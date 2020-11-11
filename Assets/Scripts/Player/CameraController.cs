using MadMaps.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
	[Header("Focus")]
	public float FocusSpeed = 3;
	public LayerMask FocusMask = ~1;
	public Vector3 FocusOffset = new Vector3(1, 0, 0);
	public Vector3 FocusPoint;
	public Vector3 m_targetFocusPoint;
	public float FocusDistance = 10;

	[Space]
	[Header("Camera")]
	public float CameraCastSize = 1;
	public Vector2 ZoomLimits = new Vector2(5, 10);
	public float LookSensitivity = 1;
	public Vector2 LookAngle;

	public bool IsFreeLook { get; private set; }

	[Header("References")]
	public PlayerInput Input;

	private Vector3 SmoothPos => Input.GetComponent<MovementController>().SmoothPosition.SmoothPosition;
	private Camera m_camera;
	private float m_currentZoom;
	private InputAction m_look, m_zoom, m_freelook;

	private void Start()
	{
		m_look = Input.actions.Single(a => a.name == "Look");
		m_zoom = Input.actions.Single(a => a.name == "Zoom");
		m_freelook = Input.actions.Single(a => a.name == "FreeLook");
		m_camera = GetComponent<Camera>();
	}

	private void Update()
	{
		IsFreeLook = m_freelook.ReadValue<float>() > 0;

		var lookDelta = m_look.ReadValue<Vector2>() * LookSensitivity * Time.deltaTime;
		m_currentZoom = Mathf.Clamp(m_currentZoom + m_zoom.ReadValue<float>(), ZoomLimits.x, ZoomLimits.y);

		LookAngle += lookDelta;
		var rotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
		transform.localPosition = rotation * Vector3.back * m_currentZoom;

		var worldFocusPos = Input.transform.localToWorldMatrix.MultiplyPoint3x4(FocusOffset);
		DebugHelper.DrawPoint(worldFocusPos, .2f, Color.cyan, 0);

		var scaleFactor = transform.lossyScale.x;
		var distFromPlayerToCamera = (transform.position - SmoothPos).magnitude;
		var obstacleRay = new Ray(SmoothPos, (transform.position - SmoothPos));
		Debug.DrawRay(obstacleRay.origin, obstacleRay.direction * FocusDistance, Color.gray);
		if (Physics.SphereCast(obstacleRay, CameraCastSize * scaleFactor, out var hit, distFromPlayerToCamera, FocusMask))
		{
			Debug.DrawLine(obstacleRay.origin, hit.point, Color.magenta);
			transform.position = hit.point + hit.normal * .1f;
		}

		var fdist = FocusDistance * scaleFactor;
		var focusRay = new Ray(transform.position, transform.forward * fdist);

		m_targetFocusPoint = focusRay.origin + focusRay.direction * fdist;
		Debug.DrawRay(focusRay.origin, focusRay.direction * fdist, Color.grey);
		if (Physics.SphereCast(focusRay, CameraCastSize * scaleFactor, out hit, fdist, FocusMask))
		{
			Debug.DrawLine(focusRay.origin, hit.point, Color.green);
			m_targetFocusPoint = hit.point;
		}
		FocusPoint = Vector3.Lerp(FocusPoint, m_targetFocusPoint, Time.deltaTime * FocusSpeed);

		transform.LookAt(worldFocusPos);
		m_camera.nearClipPlane = .3f * scaleFactor;
		m_camera.farClipPlane = 5000 * scaleFactor;

		while (LookAngle.x < -180) LookAngle.x += 360;
		while (LookAngle.x > 180) LookAngle.x -= 360;

		while (LookAngle.y < -90) LookAngle.y += 180;
		while (LookAngle.x > 90) LookAngle.y -= 180;
	}
}
