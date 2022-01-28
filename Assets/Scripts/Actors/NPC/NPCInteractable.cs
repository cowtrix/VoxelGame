using Actors;
using System.Collections.Generic;

namespace Interaction
{
	public class NPCInteractable : Interactable
	{
		public NPCActor Self;
		public override string DisplayName => Self.DisplayName;

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (Self.CanTalkTo(context))
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Talk" };
			}
		}

		public override void ExecuteAction(Actor instigator, ActorAction action)
		{
			Self.InteractWithActor(instigator, action);
			base.ExecuteAction(instigator, action);
		}
	}
}