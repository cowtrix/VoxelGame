using Actors;
using Interaction.Items;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class EquippableItemBase : Item, IEquippableItem
	{
		public Vector3 EquippedOffset, EquippedRotation;
		public Actor EquippedActor { get; private set; }

		public virtual void OnEquip(Actor actor)
		{
			EquippedActor = actor;
			transform.SetParent(actor.EquippedItemTransform);
			transform.localPosition = EquippedOffset;
			transform.localRotation = Quaternion.Euler(EquippedRotation);
			gameObject.SetActive(true);
			var rb = Rigidbody;
			if (rb)
			{
				rb.isKinematic = true;
				rb.detectCollisions = false;
				rb.position = actor.EquippedItemTransform.position;
			}
			var colliders = GetComponentsInChildren<Collider>();
			foreach(var c in colliders)
			{
				c.enabled = false;
			}
		}

		public virtual void OnUnequip(Actor actor)
		{
			EquippedActor = null;
			transform.SetParent(actor.State.InventoryContainer);
			gameObject.SetActive(false);
			var colliders = GetComponentsInChildren<Collider>();
			foreach (var c in colliders)
			{
				c.enabled = true;
			}
		}

		public virtual void OnEquipThink(Actor actor)
		{
			transform.localPosition = EquippedOffset;
			transform.localRotation = Quaternion.Euler(EquippedRotation);
		}

		public virtual void UseOn(Actor playerInteractionManager, GameObject target)
		{
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
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
			if (!EquippedActor)
			{
				base.ReceiveAction(actor, action);
			}
		}

		public override IEnumerable<ActorAction> GetActions(Actor actor)
		{
			var hasInventory = actor.GetComponentInChildren<Phone.PhoneController>();
			if (EquippedActor)
			{
				if (hasInventory)
				{
					yield return new ActorAction { Key = eActionKey.EXIT, Description = "Put Away" };
				}
				yield break;
			}
			if (hasInventory)
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Pick Up" };
				yield return new ActorAction { Key = eActionKey.EQUIP, Description = "Equip" };
			}
			else
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Pick Up" };
			}
		}

		public override bool CanUse(Actor context)
		{
			if (EquippedActor)
			{
				return false;
			}
			return base.CanUse(context);
		}
	}
}