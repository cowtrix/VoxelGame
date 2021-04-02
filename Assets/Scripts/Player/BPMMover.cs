using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMMover : MonoBehaviour
{
    public Vector3 Magnitude;
    public Vector3 Offset;

	private void Update()
	{
		transform.localPosition = Offset + BeatManager.Instance.BPMSawtooth * Magnitude;
	}
}
