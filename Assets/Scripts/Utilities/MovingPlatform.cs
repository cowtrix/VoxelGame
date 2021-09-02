using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : ExtendedMonoBehaviour
{
	private Rigidbody Rigidbody => GetComponentInParent<Rigidbody>();
	private List<Rigidbody> m_rigidBodies = new List<Rigidbody>();
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
			m_rigidBodies.Add(rb);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var rb = other.GetComponent<Rigidbody>() ?? other.GetComponentInParent<Rigidbody>();
		if (rb && !rb.isKinematic)
		{
			Debug.Log("Remove rigidbody");
			m_rigidBodies.Remove(rb);
		}
	}

	private void FixedUpdate()
	{
		var vel = Rigidbody.position - m_lastPosition;
		m_lastPosition = Rigidbody.position;
		foreach (var rb in m_rigidBodies)
		{
			Debug.Log($"Adding velocity {vel} to {rb}", rb);
			Debug.DrawLine(rb.position, rb.position + vel, Color.green);
			rb.MovePosition(rb.position + vel);
		}
	}
}