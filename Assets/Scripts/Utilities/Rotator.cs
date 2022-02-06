using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 Rotation;

    private Rigidbody m_rigidBody;

	private void Awake()
	{
		m_rigidBody = GetComponent<Rigidbody>();
	}

	void Update()
    {
		var scaledRot = Rotation * Time.deltaTime;
		if (!m_rigidBody)
		{
			transform.localRotation *= Quaternion.Euler(scaledRot);
			return;
		}
		m_rigidBody.AddTorque(scaledRot);
    }
}
