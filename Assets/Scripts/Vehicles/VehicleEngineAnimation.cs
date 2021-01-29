using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngineAnimation : MonoBehaviour
{
	public ParticleSystem ParticleSystem => GetComponentInChildren<ParticleSystem>();
	public Vector3 IdleAngle = new Vector3(5, 0, 0);

	public Vector2 EmissionRange = new Vector2(10, 64);
	private float m_targetEmission;

	private Vector3 m_lastMove;

	private void Awake()
	{
		m_targetEmission = EmissionRange.x;
	}

	public void OnMove(Vector3 vec)
	{
		m_lastMove = vec;
	}

	private void Update()
	{
		var angle = IdleAngle;

		//angle.Scale(transform.localScale);
		transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time) * angle);

		var emission = ParticleSystem.emission;
		emission.rateOverTime = Mathf.MoveTowards(emission.rateOverTime.constant, m_targetEmission, Time.deltaTime * (EmissionRange.y - EmissionRange.x));
	}
}
