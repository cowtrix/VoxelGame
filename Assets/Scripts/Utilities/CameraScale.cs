using UnityEngine;

public class CameraScale : MonoBehaviour
{
	Camera m_camera;
	float m_farClip, m_nearClip;

	private void Awake()
	{
		m_camera = GetComponent<Camera>();
		m_farClip = m_camera.farClipPlane;
		m_nearClip = m_camera.nearClipPlane;
	}

	private void Update()
	{
		var scale = transform.lossyScale.x;
		m_camera.farClipPlane = m_farClip * scale;
		m_camera.nearClipPlane = m_nearClip * scale;
	}
}