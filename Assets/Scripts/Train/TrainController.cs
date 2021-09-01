using System.Collections.Generic;
using UnityEngine;
using Voxul;

[ExecuteAlways]
public class TrainController : ExtendedMonoBehaviour
{
	public float Speed = 1;
	[Range(0, 1)]
	public float Time = 0;
	public Trainline Trainline;
	public Vector3 Rotation;

	public List<TrainController> Carriages;
	public float Space = .1f;

	private Rigidbody Rigidbody => GetComponent<Rigidbody>();

	private void Update()
	{
		var p = Trainline.GetPointOnLine(Time);
		Rigidbody.position = p.Item1;
		Rigidbody.rotation = Quaternion.LookRotation((transform.position - p.Item2).normalized) * Quaternion.Euler(Rotation);
		//Rigidbody.LookAt(p.Item2);
		//transform.localRotation *= Quaternion.Euler(Rotation);

		for (int i = 0; i < Carriages.Count; i++)
		{
			var carriage = Carriages[i];
			carriage.Time = Time - (i + 1) * Space;
		}
	}
}
