using Actors;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Interaction.Items
{
	public class DistanceLockedItem : Item, IEquippableItem
	{
		public float MaxUseDistance = 3;
		public Transform Home;

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			base.ExecuteAction(actor, action);
			if (action.Key == eActionKey.USE)
			{
				StartCoroutine(ThinkEquipped(actor));
			}
		}

		IEnumerator ThinkEquipped(Actor actor)
		{
			var waiter = new WaitForSeconds(1);
			while (actor.State.EquippedItem == this && Vector3.Distance(transform.position, Home.position) < MaxUseDistance)
			{
				yield return waiter;
			}
			if (actor.State.Inventory.Contains(this))
			{
				actor.State.DropItem(this, Home.position, Home.rotation);
			}
		}

		public override void OnDrop(Actor actor)
		{
			base.OnDrop(actor);
			transform.SetParent(Home);
		}

		public void OnEquip(Actor actor)
		{
			//transform.localPosition = EquippedOffset;
			//transform.localRotation = Quaternion.Euler(EquippedRotation);
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
