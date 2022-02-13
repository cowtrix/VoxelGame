using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
	private Vector3 m_lastGravity;

	private void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();
		Rigidbody.useGravity = false;
	}

	void FixedUpdate()
    {
		if (Rigidbody.IsSleeping())
		{
			return;
		}
		m_lastGravity = GravityManager.Instance.GetGravityForce(transform.position);
		Rigidbody.AddForce(m_lastGravity * Time.fixedDeltaTime, ForceMode.Force);
    }
}
