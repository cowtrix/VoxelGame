using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : SlowUpdater
{
    const float WakeupDistance = .2f;
    const int SleepFrameCount = 120;

    public Rigidbody Rigidbody { get; private set; }
    public CameraController CameraController { get; private set; }
    public float MaxUpdateDistance = 1000;
    public bool SleepOnStart;
    public float GravityMultiplier = 1;
    private Vector3 m_lastGravity;
    private GravityManager m_gravityManager;

    const int HISTORY_SIZE = 10;

    private SmoothPositionVector3 m_posHistory, m_rotHistory;
    private int m_sleepFrameCounter;

    public override float GetThinkSpeed() => .1f;

    private void Start()
    {
        if (CameraController.HasInstance())
        {
            CameraController = CameraController.Instance;
        }
        m_gravityManager = GravityManager.Instance;

        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;
        Rigidbody.sleepThreshold = 1;

        if (SleepOnStart)
        {
            Rigidbody.Sleep();
        }
        else
        {
            Tick(0);
        }

        m_posHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
        m_rotHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
    }

    void FixedUpdate()
    {
        if (!CameraController)
        {
            return;
        }
        m_posHistory.Push(Rigidbody.position);
        m_rotHistory.Push(Rigidbody.rotation.eulerAngles);
        if ((CameraController.transform.position - transform.position).sqrMagnitude < MaxUpdateDistance &&
            m_lastGravity.sqrMagnitude > 0 &&
            (m_posHistory.SmoothPosition - transform.position).sqrMagnitude < WakeupDistance * WakeupDistance)
        {
            m_sleepFrameCounter = 0;
            Rigidbody.WakeUp();
            Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else
        {
            m_sleepFrameCounter++;
        }
        if (m_sleepFrameCounter > SleepFrameCount)
        {
            Rigidbody.Sleep();
            m_lastGravity = default;
        }
    }

    protected override int Tick(float dt)
    {
        if (!m_gravityManager)
        {
            if (GravityManager.HasInstance())
            {
                m_gravityManager = GravityManager.Instance;
            }
            else
            {
                return 0;
            }
        }
        m_lastGravity = m_gravityManager.GetGravityForce(transform.position) * GravityMultiplier;
        return 1;
    }
}
