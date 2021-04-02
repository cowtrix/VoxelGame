using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentChaser : MonoBehaviour
{
    public Transform Chaser;
    public float Distance = 1;
	public float ChaseSpeed = 1f;

	private Vector3 ChasePosition => Chaser.transform.position + 
		Chaser.transform.forward * Distance;

	private void Update()
	{
		var dt = Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position, ChasePosition, ChaseSpeed * dt);
		transform.LookAt(Chaser);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, ChasePosition);
	}
}
