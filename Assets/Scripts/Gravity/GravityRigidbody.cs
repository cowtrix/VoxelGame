using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRigidbody : MonoBehaviour
{
    private Rigidbody Rigidbody => GetComponent<Rigidbody>();

	private void Start()
	{
		Rigidbody.useGravity = false;
	}

	void FixedUpdate()
    {
        Rigidbody.AddForce(GravityManager.Instance.GetGravityForce(transform.position) * Time.fixedDeltaTime);
    }
}
