using Actors;
using Interaction.Items;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public abstract class EquippableItemBase : Item, IEquippableItem
	{
		public Actor EquippedActor { get; private set; }

		public virtual void OnEquip(Actor actor)
		{
			EquippedActor = actor;
			transform.SetParent(actor.Settings.EquippedItemTransform);
			gameObject.SetActive(true);
		}

		public virtual void OnUnequip(Actor actor)
		{
			EquippedActor = null;
			transform.SetParent(actor.State.InventoryContainer);
			gameObject.SetActive(false);
		}

		public virtual void UseOn(Actor playerInteractionManager, GameObject target)
		{
		}

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			if(action.Key == eActionKey.EQUIP)
			{
				actor.State.EquipItem(this);
				return;
			}
			if(action.Key == eActionKey.EXIT)
			{
				actor.State.EquipItem(null);
				return;
			}
			base.ExecuteAction(actor, action);
		}

		public override IEnumerable<ActorAction> GetActions(Actor actor)
		{
			if (EquippedActor)
			{
				yield return new ActorAction { Key = eActionKey.EXIT, Description = "Stop Using" };
				yield break;
			}
			yield return new ActorAction { Key = eActionKey.USE, Description = "Pick Up" };
			yield return new ActorAction { Key = eActionKey.EQUIP, Description = "Equip" };
		}
	}
}