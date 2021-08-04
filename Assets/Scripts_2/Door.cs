using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Door : MonoBehaviour
{
	[Range(0, 1)]
	public float OpenAmount;
    public Vector3 OpenPosition, OpenRotation;
    public Vector3 ClosedPosition, ClosedRotation;

	private void Reset()
	{
		OpenPosition = transform.localPosition;
		ClosedPosition = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = Vector3.Lerp(ClosedPosition, OpenPosition, OpenAmount);
		transform.localRotation = Quaternion.Lerp(Quaternion.Euler(ClosedRotation), Quaternion.Euler(OpenRotation), OpenAmount);
	}
}
