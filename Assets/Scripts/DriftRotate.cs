using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftRotate : MonoBehaviour
{
    public Vector3 Rot;
	public float DriftSpeed = .1f;

	private void Awake()
	{
		Rot = transform.forward;
	}

	void Update()
    {
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Rot), DriftSpeed * Time.deltaTime);
		Rot += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * DriftSpeed * Time.deltaTime;
		Rot = Rot.normalized;
	}
}
