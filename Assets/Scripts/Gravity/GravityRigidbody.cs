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
    private Vector3 m_lastGravity, m_lastPosition;
    private Quaternion m_lastRotation;
    private GravityManager m_gravityManager;

    const int HISTORY_SIZE = 10;

    private SmoothPositionVector3 m_posHistory, m_rotHistory;
    private int m_sleepFrameCounter;

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

        m_posHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);
        m_rotHistory = new SmoothPositionVector3(HISTORY_SIZE, transform.position);

        if (SleepOnStart)
        {
            Rigidbody.Sleep();
        }
        else
        {
            TickOnThread(0);
        }
    }

    void FixedUpdate()
    {
        if (!CameraController)
        {
            return;
        }
        if(m_lastGravity.sqrMagnitude > 0)
        {
            Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        m_lastPosition = Rigidbody.position;
        m_lastRotation = Rigidbody.rotation;
    }

    protected override int TickOnThread(float dt)
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
        
        if ((CameraController.transform.position - transform.position).sqrMagnitude < MaxUpdateDistance &&
            m_lastGravity.sqrMagnitude > 0 &&
            (m_posHistory.SmoothPosition - transform.position).sqrMagnitude < WakeupDistance * WakeupDistance)
        {
            m_sleepFrameCounter = 0;
            Rigidbody.WakeUp();
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
        
        return 1;
    }

    protected override void TickOffThread(float dt)
    {
        m_posHistory.Push(m_lastPosition);
        m_rotHistory.Push(m_lastRotation.eulerAngles);
        m_lastGravity = m_gravityManager.GetGravityForce(m_lastPosition) * GravityMultiplier;
    }
}
