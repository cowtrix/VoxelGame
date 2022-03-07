using UnityEngine;

public class SinMover : MonoBehaviour
{
	public float Speed = 1;
	public bool RotationEnabled = true, PositionEnabled = true, RandomYSpin;
	public Vector3 StartPosition, StartRotation;
	public Vector3 EndPosition, EndRotation;

	private void Reset()
	{
		StartPosition = transform.localPosition;
		StartRotation = transform.localRotation.eulerAngles;
		EndPosition = transform.localPosition;
		EndRotation = transform.localRotation.eulerAngles;
	}

	private void Start()
	{
		if (RandomYSpin)
		{
			var randY = Random.Range(0, 360);
			StartRotation.y = randY;
			EndRotation.y = randY;
		}
	}

	private void Update()
	{
		var t = (Mathf.Sin(Time.time * Speed) + 1) / 2;
		if (PositionEnabled)
		{
			transform.localPosition = Vector3.Lerp(EndPosition, StartPosition, t);
		}
		if (RotationEnabled)
		{
			transform.localRotation = Quaternion.Euler(Vector3.Lerp(EndRotation, StartRotation, t)); 
		}
	}
}