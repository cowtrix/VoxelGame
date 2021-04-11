using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wurm : EnemyObject
{
    public float WanderDistance = 20;

    public Transform Head;
    public LayerMask CollisionMask;
    public float ObstacleCastDistance = 5;
    public float MoveSpeed = 10;
    public float RotateSpeed = 1;

    private Vector3 m_targetPosition;
    private Quaternion m_targetHeading;
    private float m_lastCoordinateUpdate;

    protected override void Update()
    {
        base.Update();
        UpdateTargetPosition();
        
        var dt = Time.deltaTime;
        var headingVector = m_targetPosition - transform.position;
        if (headingVector.sqrMagnitude == 0)
        {
            // get nans and infinity
            return;
        }
        m_targetHeading = Quaternion.LookRotation(headingVector.normalized, transform.up);

        transform.rotation = Quaternion.identity;
        Head.rotation = Quaternion.RotateTowards(Head.rotation, m_targetHeading, RotateSpeed * dt);
        transform.position += Head.forward * MoveSpeed * dt;
    }

    Vector3 GetVacantDirection(Vector3 normal)
	{
        var vec = new[] { normal, transform.up, transform.right, -transform.right, -transform.up };
        foreach(var v in vec)
		{
            if (!Physics.Raycast(transform.position, v, ObstacleCastDistance, CollisionMask))
            {
                Debug.DrawRay(transform.position, v, Color.green, 4);
                return v;
            }
            Debug.DrawRay(transform.position, v, Color.red);
        }
        return -Head.forward;
    }

    void UpdateTargetPosition()
	{
        m_lastCoordinateUpdate -= Time.deltaTime;
        if (Physics.Raycast(transform.position, Head.forward, out var hit, ObstacleCastDistance, CollisionMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            // Something in front forward
            var vacantDirection = GetVacantDirection((Head.forward + hit.normal) / 2f);
            m_targetPosition = transform.position + vacantDirection * ObstacleCastDistance;
            //Debug.Log("Steered");
            return;
        }
		else
		{
            if (m_lastCoordinateUpdate < 0 ||
                (transform.position - m_targetPosition).sqrMagnitude < 1f)
            {
                var wanderDistance = WanderDistance / 2f;
                m_targetPosition = CameraController.Instance.transform.position + new Vector3(
                    Random.Range(-1f, 1f) * wanderDistance,
                    Random.Range(-1f, 1f) * wanderDistance,
                    Random.Range(-1f, 1f) * wanderDistance);

                m_lastCoordinateUpdate = 2;
            }
        }
    }

	private void OnDrawGizmos()
	{
        Gizmos.DrawLine(transform.position, m_targetPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + m_targetHeading * transform.forward);
    }
}
