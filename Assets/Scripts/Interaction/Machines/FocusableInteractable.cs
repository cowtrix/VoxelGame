using Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction.Activities
{
	[Serializable]
	public struct RotationLimits
	{
		public Vector2 X, Y, Z;

		public Quaternion ClampRotation(Quaternion rot) =>
			Quaternion.Euler(Mathf.Clamp(rot.eulerAngles.x, X.x, X.y), Mathf.Clamp(rot.eulerAngles.y, Y.x, Y.y), Mathf.Clamp(rot.eulerAngles.z, Z.x, Z.y));
	}

	public abstract class FocusableInteractable : Interactable, ICameraControllerProxy
	{
		public Quaternion? LookDirectionOverride => transform.localToWorldMatrix.rotation * Quaternion.Euler(LookRotation + m_additionalRotation);
		public Vector3? LookPositionOverride => transform.localToWorldMatrix.MultiplyPoint(LookOffset);

		public RotationLimits CameraRotationLimits;
		public float LookSpeed = 1;
		public Vector3 LookOffset, LookRotation;

		protected Vector3 m_additionalRotation;

		public Actor Actor { get; protected set; }

		public ActorEvent OnActivate, OnDeactivate;

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (!CanUse(context))
				yield break;
			if (CameraController.Instance.Proxy == this as ICameraControllerProxy)
			{
				yield return new ActorAction { Key = eActionKey.EXIT, Description = "Stop Using" };
				yield break;
			}
			yield return new ActorAction { Key = eActionKey.USE, Description = "Start Using" };
		}

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			switch (action.Key)
			{
				case eActionKey.USE:
					if (!Actor)
					{
						Actor = actor;
						CameraController.Instance.Proxy = this;
						OnActivate?.Invoke(actor);
						break;
					}
					base.ExecuteAction(actor, action);
					break;
				case eActionKey.EXIT:
					Actor = null;
					OnDeactivate?.Invoke(actor);
					if (CameraController.Instance.Proxy != this as ICameraControllerProxy)
					{
						return;
					}
					CameraController.Instance.Proxy = null;
					break;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(LookOffset, Vector3.one * .1f);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(LookRotation) * Vector3.forward);
		}

		public virtual void Look(Actor actor, Vector2 lastDelta) { }
	}
}