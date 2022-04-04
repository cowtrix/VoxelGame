using Actors;
using Interaction.Activities;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
	public class NPCInteractable : Activity
	{
		public override Vector3? LookPositionOverride => null;
		public override Quaternion? LookDirectionOverride => null;

		public NPCActor Self;
		public override string DisplayName => Self.DisplayName;

		protected override void Start()
		{
			base.Start();
			Self.Controller.OnFinished.AddListener(() => Actor?.TryStopActivity(this));
		}

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if(Actor == context)
			{
				yield return new ActorAction { Key = eActionKey.EXIT, Description = "Stop Talking" };
			}
			else
			{
				if (Self.CanTalkTo(context))
				{
					yield return new ActorAction { Key = eActionKey.USE, Description = "Talk" };
				}
			}
		}

		public override void OnStartActivity(Actor actor)
		{
			if (!Actor)
			{
				Self.InteractWithActor(actor);
			}
			base.OnStartActivity(actor);
		}

		public override void OnStopActivity(Actor actor)
		{
			if(Actor == actor)
			{
				Self.StopInteractingWithActor(Actor);
			}
			base.OnStopActivity(actor);
		}
	}
}