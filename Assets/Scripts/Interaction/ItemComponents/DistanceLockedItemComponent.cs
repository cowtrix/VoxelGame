using Actors;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Interaction.Items
{
	public class DistanceLockedItemComponent : ItemComponent
	{
		public float MaxUseDistance = 3;
		public Transform Home;

        public bool EquipOnPickup { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override bool ReceiveAction(Actor actor, ActorAction action)
		{
			base.ReceiveAction(actor, action);
			if (action.State == eActionState.End && action.Key == eActionKey.USE)
			{
				StartCoroutine(ThinkEquipped(actor));
				return true;
			}
			return false;
		}

		IEnumerator ThinkEquipped(Actor actor)
		{
			var waiter = new WaitForSeconds(1);
			while ((Object)actor.State.EquippedItem == this && Vector3.Distance(transform.position, Home.position) < MaxUseDistance)
			{
				yield return waiter;
			}
			if (actor.State.Inventory.Contains(Item))
			{
				actor.State.DropItem(Item, Home.position, Home.rotation);
			}
		}

		public override void OnDrop(Actor actor)
		{
			base.OnDrop(actor);
			transform.SetParent(Home);
		}

		public void OnEquip(Actor actor)
		{
		}

		public void OnUnequip(Actor actor)
		{
		}

		public void UseOn(Actor playerInteractionManager, GameObject target)
		{
		}

		public void OnEquipThink(Actor actorState)
		{
		}
	}
}
