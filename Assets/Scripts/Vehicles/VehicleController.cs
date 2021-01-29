using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class VectorEvent : UnityEvent<Vector3> { }

public class VehicleController : MonoBehaviour
{
	public Vector3 DriverPosition;
	public float DriverScaleFactor = 1;

	public float DriftMagnitude = .01f;
	public float DriftFrequency = 10f;

	public float Thrust = 1;
	[Range(0, 0.999f)]
	public float ThrustFalloff = 0.8f;
	public Vector3 ThrustVector;
	public VectorEvent OnMove;

	private Vector3 m_driftVector = Vector3.up;

	private Rigidbody m_rigidbody => gameObject.GetComponent<Rigidbody>();

	public void Move(Vector3 direction)
	{
		ThrustVector = direction.normalized;
	}

	private void FixedUpdate()
	{
		var driftForce = Mathf.Sin(Time.time * DriftFrequency) * DriftMagnitude * Vector3.up;
		m_rigidbody.AddForce(driftForce);
		m_rigidbody.AddForce(ThrustVector * Thrust * Time.fixedDeltaTime, ForceMode.Acceleration);
		ThrustVector *= Time.fixedDeltaTime * ThrustFalloff;
		OnMove.Invoke(ThrustVector);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(DriverPosition, Vector3.one * DriverScaleFactor);
	}
}
