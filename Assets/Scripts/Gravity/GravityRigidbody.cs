using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : SlowUpdater
{
	public Rigidbody Rigidbody { get; private set; }
	public CameraController CameraController { get; private set; }
	public float MaxUpdateDistance = 1000;

	public float GravityMultiplier = 1;
	private Vector3 m_lastGravity;
	private GravityManager m_gravityManager;

	const int HISTORY_SIZE = 10;

	private SmoothPositionVector3 m_posHistory, m_rotHistory;

	public override float GetThinkSpeed() => .1f;

    private void Start()
	{
        if (CameraController.HasInstance())
        {
			CameraController = CameraController.Instance;
		}
        if (GravityManager.HasInstance())
        {
			m_gravityManager = GravityManager.Instance;
		}

		Rigidbody = GetComponent<Rigidbody>();
		Rigidbody.useGravity = false;
		Rigidbody.sleepThreshold = 1;

		m_posHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
		m_rotHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
	}

	void FixedUpdate()
	{
        if (!CameraController)
        {
			return;
        }
		if ((CameraController.transform.position - transform.position).sqrMagnitude < MaxUpdateDistance && m_lastGravity.sqrMagnitude > 0)
		{
			Rigidbody.WakeUp();
			Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Force);
			m_lastGravity = Vector3.zero;
		}
	}

	protected override int Tick(float dt)
	{
		if (!m_gravityManager)
		{
			return 0;
		}
		m_lastGravity = m_gravityManager.GetGravityForce(transform.position) * GravityMultiplier;
		m_posHistory.Push(Rigidbody.position);
		m_rotHistory.Push(Rigidbody.rotation.eulerAngles);
		const float threshold = .5f;
		//m_positionChangedRecently = m_posHistory.Count == m_posHistory.Capacity;
		if ((m_posHistory.SmoothPosition - transform.position).sqrMagnitude < threshold * threshold)
		{
			//m_positionChangedRecently = false;
			Rigidbody.Sleep();
		}
		return 1;
	}
}
