using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : MonoBehaviour
{
    private Rigidbody Rigidbody => GetComponent<Rigidbody>();
	private Vector3 m_lastGravity;

	private void Start()
	{
		Rigidbody.useGravity = false;
	}

	void FixedUpdate()
    {
		m_lastGravity = GravityManager.Instance.GetGravityForce(transform.position);
		Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Force);
    }

	private void OnDrawGizmosSelected()
	{
		//Gizmos.DrawLine(transform.position, transform.position + m_lastGravity);
		//GizmoExtensions.Label(transform.position + m_lastGravity, $"g={m_lastGravity}");
	}
}
