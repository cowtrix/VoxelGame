using Actors;
using Actors.NPC.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul.Utilities;

namespace Interaction.Activities
{
	[Serializable]
	public struct RotationLimits
	{
		public Vector2 X, Y, Z;

		public Quaternion ClampRotation(Quaternion rot) =>
			Quaternion.Euler(ClampRotation(rot.eulerAngles));

		public Vector3 ClampRotation(Vector3 rot) =>
			new Vector3(Mathf.Clamp(rot.x, X.x, X.y), Mathf.Clamp(rot.y, Y.x, Y.y), Mathf.Clamp(rot.z, Z.x, Z.y));
	}

	public abstract class Activity : Interactable, ICameraControllerProxy
	{
		public virtual Quaternion? LookDirectionOverride => transform.localToWorldMatrix.rotation * Quaternion.Euler(LookRotation + m_additionalRotation);
		public virtual Vector3? LookPositionOverride => transform.localToWorldMatrix.MultiplyPoint(LookOffset);
		public Vector2 LookAngle { get; private set; }

		[Header("Camera")]
		public RotationLimits CameraRotationLimits;
		public float LookSpeed = 1;
		public Vector3 LookOffset, LookRotation;

		protected Vector3 m_additionalRotation;

		public Actor Actor { get; private set; }

		public ActorEvent OnActivate, OnDeactivate;
		public bool PassThroughInteraction;

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
			if(Actor == context)
            {
				yield return new ActorAction { Key = eActionKey.MOVE };
			}
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
		{
			if(action.State != eActionState.End)
			{
				return;
			}
			if(Actor == actor)
			{
				if (action.Key == eActionKey.LOOK)
				{
					Look(actor, action.Context);
					return;
				}
			}
			switch (action.Key)
			{
				case eActionKey.USE:
					if (!Actor)
					{
						actor.TryStartActivity(this);
						break;
					}
					base.ReceiveAction(actor, action);
					break;
				case eActionKey.EXIT:
					Actor.TryStopActivity(this);
					break;
			}
		}

		protected override void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(LookOffset, Vector3.one * .1f);
			Gizmos.color = Color.green;

			var forward = Quaternion.Euler(LookRotation) * Vector3.forward;
			Gizmos.DrawLine(LookOffset, LookOffset + forward);

			// Draw camera rotation limits
			Gizmos.color = Color.grey;
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.x, CameraRotationLimits.Y.x, CameraRotationLimits.Z.x) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.x, CameraRotationLimits.Y.y, CameraRotationLimits.Z.x) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.x, CameraRotationLimits.Y.x, CameraRotationLimits.Z.y) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.x, CameraRotationLimits.Y.y, CameraRotationLimits.Z.y) * forward);

			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.y, CameraRotationLimits.Y.x, CameraRotationLimits.Z.x) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.y, CameraRotationLimits.Y.y, CameraRotationLimits.Z.x) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.y, CameraRotationLimits.Y.x, CameraRotationLimits.Z.y) * forward);
			Gizmos.DrawLine(LookOffset, LookOffset + Quaternion.Euler(CameraRotationLimits.X.y, CameraRotationLimits.Y.y, CameraRotationLimits.Z.y) * forward);
			base.OnDrawGizmosSelected();
		}

		public virtual void OnStartActivity(Actor actor)
		{
			Actor = actor;
			if(Actor is PlayerActor player)
			{
				player.CameraController.Proxy = this;
			}
			OnActivate?.Invoke(actor);
		}

		public virtual void OnStopActivity(Actor actor)
		{
			if (Actor is PlayerActor player && (UnityEngine.Object)player.CameraController.Proxy == this)
			{
				player.CameraController.Proxy = null;
			}
			Actor = null;
			OnDeactivate?.Invoke(actor);
			m_additionalRotation = default;
		}

		public virtual void Look(Actor actor, Vector2 lastDelta)
		{
			m_additionalRotation = CameraRotationLimits.ClampRotation(m_additionalRotation + new Vector3(-lastDelta.y, lastDelta.x) * LookSpeed * Time.deltaTime);
		}

	}
}