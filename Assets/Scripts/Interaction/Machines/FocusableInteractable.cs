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
					OnActivate?.Invoke(actor);
					base.Use(actor, action);
					break;
				case STOP_USE:
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

		public virtual void Fire(PlayerActor playerActor) { }

		public virtual void Move(Actor actor, Vector2 direction) { }

		public virtual void Look(Actor actor, Vector2 direction)
		{
			direction *= Time.deltaTime * LookSpeed;
			var currentRot = LookRotation + m_additionalRotation;
			var newRot = new Vector3(Mathf.Clamp(currentRot.x - direction.y, CameraRotationLimits.X.x, CameraRotationLimits.X.y),
				Mathf.Clamp(currentRot.y + direction.x, CameraRotationLimits.Y.x, CameraRotationLimits.Y.y),
				Mathf.Clamp(currentRot.z, CameraRotationLimits.Z.x, CameraRotationLimits.Z.y));
			m_additionalRotation = newRot - LookRotation;
		}

		protected virtual void OnActiveUse(Actor actor, string action) { }

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(LookOffset, Vector3.one * .1f);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(LookRotation) * Vector3.forward);
		}
	}
}