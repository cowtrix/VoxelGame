using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : SlowUpdater
{
	public Rigidbody Rigidbody { get; private set; }
	private Vector3 m_lastGravity;

	const int HISTORY_SIZE = 10;

	private SmoothPositionVector3 m_posHistory, m_rotHistory;

	private void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();
		Rigidbody.useGravity = false;
		Rigidbody.sleepThreshold = 1;

		m_posHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
		m_rotHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
	}

	void FixedUpdate()
	{
		if (Rigidbody.IsSleeping())
		{
			return;
		}
		m_lastGravity = GravityManager.Instance.GetGravityForce(transform.position);
		if (m_lastGravity.sqrMagnitude > 0)
		{
			Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Force);
		}
		m_posHistory.Push(Rigidbody.position);
		m_rotHistory.Push(Rigidbody.rotation.eulerAngles);
	}

	protected override void Tick(float dt)
	{
		const float threshold = .5f;
		if ((m_posHistory.SmoothPosition - transform.position).sqrMagnitude < threshold * threshold)
		{
			Rigidbody.Sleep();
		}
	}
}
