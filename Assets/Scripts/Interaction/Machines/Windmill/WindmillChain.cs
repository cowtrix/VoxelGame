using System.Collections;
using UnityEngine;
using Voxul;

public class WindmillChain : RenderBehaviour
{
	public float Rotation;
	public float RotationSpeed = 1;
	public float Margin = 20;
	public float Torque = 1;
	public float AngleLimit = 1;

	protected override void UpdateOnScreen()
	{
		Rotation += RotationSpeed * Time.deltaTime;
		while (Rotation > 360)
		{
			Rotation -= 360;
		}
		var childCount = transform.childCount;
		var lastRotation = Rotation;
		for (var i = 0; i < childCount; ++i)
		{
			var r = transform.GetChild(i);
			var targetRot = Quaternion.Euler(0, lastRotation + 45 + Margin * Mathf.Sign(RotationSpeed), 0);
			r.transform.localRotation = Quaternion.Lerp(r.transform.localRotation, targetRot, Mathf.Clamp01(Torque * Time.deltaTime));
			if (Quaternion.Angle(r.transform.localRotation, targetRot) > AngleLimit)
			{
				break;
			}
			lastRotation = r.rotation.eulerAngles.y;
		}
	}
}
