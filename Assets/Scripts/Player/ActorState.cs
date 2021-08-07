using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : StateContainer
{
	public Vector3 Position;
	public Quaternion Rotation;

	protected virtual void Update()
	{
		Position = transform.position;
		Rotation = transform.rotation;
	}
}
