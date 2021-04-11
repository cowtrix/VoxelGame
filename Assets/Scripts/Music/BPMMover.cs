using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMMover : MonoBehaviour
{
	[Range(0,1)]
	public double TimeOffset;
    public Vector3 Magnitude;
    public Vector3 Offset;

	private void Update()
	{
		transform.localPosition = Offset + (float)BeatManager.Instance.BPMSawtooth(TimeOffset) * Magnitude;
	}
}
