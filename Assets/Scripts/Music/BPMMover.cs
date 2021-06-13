using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMMover : MonoBehaviour
{
	public float TimeOffset;
    public Vector3 Magnitude;
    public Vector3 Offset;

	public float Frequency = 1;

	private void Start()
	{
		Offset = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = Offset + Mathf.Sin((Time.time + TimeOffset) * Frequency) * Magnitude;
	}
}
