using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class ClawMachine : FocusableInteractable
{
	public override string DisplayName => name;
	public float MovementSpeed = .01f;
	public Transform Claw, ClawOrigin;

	[Range(0, 1)]
	public float X;
	[Range(0, 1)]
	public float Y;
	[Range(0, 1)]
	public float Z;
	public Bounds ClawBounds;
	public BezierConnectorLineRenderer LineRenderer;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(ClawBounds.center, ClawBounds.size);
	}

	private void Update()
	{
		var targetPosition = ClawBounds.min + new Vector3(ClawBounds.size.x * X, ClawBounds.size.y * Y, ClawBounds.size.z * Z);
		if(targetPosition == Claw.localPosition)
		{
			return;
		}
		Claw.localPosition = targetPosition;
		ClawOrigin.localPosition = targetPosition.xz().x0z(ClawOrigin.localPosition.y);
		LineRenderer.Invalidate();
	}

	public override IEnumerable<string> GetActions(Actor context)
	{
		if (!CanUse(context))
			yield break;
		if (CameraController.Instance.Proxy == this as ICameraControllerProxy)
		{
			yield return "[${Move}] Move Claw";
			yield return "[${Fire}] Pick";
			yield return STOP_USE;
			yield break;
		}
		yield return USE;
	}

	public override void Fire(PlayerActor playerActor)
	{
		
	}

	public override void Move(Actor actor, Vector2 direction)
	{
		direction *= MovementSpeed;
		X = Mathf.Clamp01(X - direction.y);
		Z = Mathf.Clamp01(Z + direction.x);
	}
}
