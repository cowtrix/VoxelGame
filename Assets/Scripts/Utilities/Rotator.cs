using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public bool RandomiseOnStart;
    public Vector3 Rotation;
	public ForceMode ForceMode;
	private Rigidbody m_rigidBody;

	private void Awake()
	{
		m_rigidBody = GetComponent<Rigidbody>();

		if (RandomiseOnStart)
		{
			transform.localRotation = Quaternion.Euler(Rotation.x * 360 * Random.value, Rotation.y * 360 * Random.value, Rotation.z * 360 * Random.value);
		}
	}

	void Update()
    {
		var scaledRot = Rotation * Time.deltaTime;
		if (!m_rigidBody || m_rigidBody.isKinematic)
		{
			transform.localRotation *= Quaternion.Euler(scaledRot);
			return;
		}
		m_rigidBody.AddTorque(scaledRot, ForceMode);
    }
}
