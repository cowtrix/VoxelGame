using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightScale : MonoBehaviour
{
	Light m_light;
	float m_range;
	float m_intensity;

	private void Awake()
	{
		m_light = GetComponent<Light>();
		m_range = m_light.range;
		m_intensity = m_light.intensity;
	}

	private void Update()
	{
		var scale = transform.lossyScale.x;
		m_light.range = m_range * scale;
		m_light.intensity = m_intensity * scale;
	}
}
