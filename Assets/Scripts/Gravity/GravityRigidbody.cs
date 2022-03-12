using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : SlowUpdater
{
	public Rigidbody Rigidbody { get; private set; }
	private Vector3 m_lastGravity;
	private GravityManager m_gravityManager;

	const int HISTORY_SIZE = 10;

	private SmoothPositionVector3 m_posHistory, m_rotHistory;

	private void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();
		Rigidbody.useGravity = false;
		Rigidbody.sleepThreshold = 1;

		m_gravityManager = GravityManager.Instance;

		m_posHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
		m_rotHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
	}

	void FixedUpdate()
	{
		if (Rigidbody.IsSleeping())
		{
			return;
		}
		if (m_lastGravity.sqrMagnitude > 0)
		{
			Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Force);
		}
	}

	protected override int Tick(float dt)
	{
		m_lastGravity = m_gravityManager.GetGravityForce(transform.position);
		m_posHistory.Push(Rigidbody.position);
		m_rotHistory.Push(Rigidbody.rotation.eulerAngles);
		const float threshold = .5f;
		if ((m_posHistory.SmoothPosition - transform.position).sqrMagnitude < threshold * threshold)
		{
			Rigidbody.Sleep();
		}
		return 1;
	}
}
