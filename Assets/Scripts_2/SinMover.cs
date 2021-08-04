using UnityEngine;

public class SinMover : MonoBehaviour
{
	public float Speed = 1;
	public Vector3 StartPosition, StartRotation;
	public Vector3 EndPosition, EndRotation;

	private void Reset()
	{
		StartPosition = transform.localPosition;
		EndPosition = transform.localPosition;
	}

	private void Update()
	{
		var t = (Mathf.Sin(Time.time * Speed) + 1) / 2;
		transform.localPosition = Vector3.Lerp(EndPosition, StartPosition, t);
		transform.localRotation = Quaternion.Lerp(Quaternion.Euler(EndRotation), Quaternion.Euler(StartRotation), t);
	}
}