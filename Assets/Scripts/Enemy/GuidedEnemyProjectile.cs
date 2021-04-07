using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuidedEnemyProjectile : MonoBehaviour
{
    public float Velocity = 20;
    protected Rigidbody Rigidbody => GetComponent<Rigidbody>();

    protected Vector3 m_targetPosition;

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var playerPos = CameraController.Instance.transform.position;
        var playerVel = CameraController.Instance.GetComponentInParent<Rigidbody>().velocity;
        m_targetPosition = playerPos; //Mathfx.CalculateInterceptCourse(playerPos, playerVel, transform.position, Velocity);

        var diffVector = m_targetPosition - transform.position;

        Rigidbody.AddForce(diffVector.normalized * Rigidbody.mass * Velocity * dt, ForceMode.Impulse);

        Debug.DrawLine(transform.position, m_targetPosition, Color.green);
    }


}
