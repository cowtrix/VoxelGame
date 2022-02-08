using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : ExtendedMonoBehaviour
{
	public Rigidbody Rigidbody;
	private List<Rigidbody> m_trackedRigidBodies = new List<Rigidbody>();
	private Vector3 m_lastPosition;

	private void Start()
	{
		m_lastPosition = Rigidbody.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		var rb = other.GetComponent<Rigidbody>() ?? other.GetComponentInParent<Rigidbody>();
		if (rb && !rb.isKinematic)
		{
			Debug.Log("Added rigidbody");
			m_trackedRigidBodies.Add(rb);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var rb = other.GetComponent<Rigidbody>() ?? other.GetComponentInParent<Rigidbody>();
		if (rb && !rb.isKinematic)
		{
			Debug.Log("Remove rigidbody");
			m_trackedRigidBodies.Remove(rb);
		}
	}

	private void FixedUpdate()
	{
		var vel = Rigidbody.position - m_lastPosition;
		m_lastPosition = Rigidbody.position;
		foreach (var rb in m_trackedRigidBodies)
		{
			Debug.Log($"Adding velocity {vel} to {rb}", rb);
			Debug.DrawLine(rb.position, rb.position + vel, Color.green);
			rb.MovePosition(rb.position + vel);
		}
	}
}