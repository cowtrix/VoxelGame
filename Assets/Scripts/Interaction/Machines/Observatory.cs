using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Activities
{
	public class Observatory : Activity
	{
		public override string DisplayName => "Observatory";
		public float RotateSpeed = 1;
		public float TiltSpeed = 1;

		public Transform TelescopeBase;
		public Transform Telescope;
		public Door TelescopeTilt;

		public Transform VizBase;
		public Transform VizTelescope;
		public float VizOffset = 90;

		private Vector2 m_move;

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (!CanUse(context))
				yield break;
			if (Actor == context)
			{
				yield return new ActorAction { Key = eActionKey.MOVE, Description = "Move Telescope" };
				yield return new ActorAction { Key = eActionKey.EXIT, Description = "Stop Using" };
				yield break;
			}
			else
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Start Using" };
			}
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
		{
			switch (action.Key)
			{
				case eActionKey.MOVE:
					var delta = action.Context;
					Move(actor, delta);
					return;
			}
			base.ReceiveAction(actor, action);
		}

		private void Update()
		{
			var dt = Time.deltaTime;
			TelescopeBase.Rotate(Vector3.up * RotateSpeed * m_move.x * dt);
			TelescopeTilt.OpenAmount = Mathf.Clamp01(TelescopeTilt.OpenAmount + TiltSpeed * -m_move.y * dt);

			VizBase.localRotation = Quaternion.Euler(0, VizOffset, 0) * TelescopeBase.localRotation;
			VizTelescope.localRotation = Telescope.localRotation;
		}

		private void Move(Actor actor, Vector2 delta)
		{
			m_move = delta;
		}
	}
}