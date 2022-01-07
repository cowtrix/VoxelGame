using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class FocusableInteractable : Interactable, ICameraControllerProxy
{
	public Quaternion? LookDirectionOverride => transform.localToWorldMatrix.rotation * Quaternion.Euler(LookRotation);
	public Vector3? LookPositionOverride => transform.localToWorldMatrix.MultiplyPoint(LookOffset);

	public Vector3 LookOffset, LookRotation;

	public Actor Actor { get; protected set; }

	public override IEnumerable<string> GetActions(Actor context)
	{
		if (!CanUse(context))
			yield break;
		if (CameraController.Instance.Proxy == this as ICameraControllerProxy)
		{
			yield return STOP_USE;
			yield break;
		}
		yield return USE;
	}

	public override void Use(Actor actor, string action)
	{
		switch (action)
		{
			case USE:
				if (Actor)
				{
					Debug.LogWarning($"Interactable was already occupied by {Actor}", Actor);
					return;
				}
				Actor = actor;
				CameraController.Instance.Proxy = this;
				base.Use(actor, action);
				break;
			case STOP_USE:
				Actor = null;
				if (CameraController.Instance.Proxy != this as ICameraControllerProxy)
				{
					return;
				}
				CameraController.Instance.Proxy = null;
				break;
		}
	}

	public virtual void Fire(PlayerActor playerActor) { }

	public virtual void Move(Actor actor, Vector2 direction) { }

	protected virtual void OnActiveUse(Actor actor, string action) { }

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(LookOffset, Vector3.one * .1f);
		Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(LookRotation) * Vector3.forward);
	}
}
