using Interaction.Items;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public class ActorState : StateContainer, ICreditConsumerActor
	{
		public enum eInventoryAction
		{
			DROP, PICKUP, EQUIP,
			UNEQUIP
		}

		public InventoryStateUpdateEvent OnInventoryUpdate = new InventoryStateUpdateEvent();
		public IEquippableItem EquippedItem
		{
			get
			{
				return __equippedItem;
			}
			private set
			{
				if(value == __equippedItem)
				{
					return;
				}
				if (__equippedItem != null)
				{
					__equippedItem?.OnUnequip(Actor);
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.UNEQUIP, __equippedItem as Item);
				}
				__equippedItem = value;
				if(__equippedItem != null)
				{
					__equippedItem?.OnEquip(Actor);
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.EQUIP, __equippedItem as Item);
				}
			}
		}
		private IEquippableItem __equippedItem;
		public IReadOnlyCollection<Item> Inventory => GetComponentsInChildren<Item>(true);
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		[StateMin(0)]
		public virtual int Credits { get; protected set; } = 6;
		public Transform InventoryContainer { get; private set; }

		protected override void Awake()
		{
			InventoryContainer = new GameObject($"{name}_Inventory").transform;
			InventoryContainer.SetParent(transform);
			base.Awake();
		}

		protected virtual void Update()
		{
			Position = transform.position;
			Rotation = transform.rotation;
		}

		protected override void OnSaveDataLoaded()
		{
			transform.position = Position;
			transform.rotation = Rotation;
		}

		public virtual bool TryPurchase(IPurchaseableItem purchaseable)
		{
			var cost = purchaseable.Cost;
			return TryAdd(nameof(Credits), -cost);
		}

		public void EquipItem(IEquippableItem equippableItem)
		{
			EquippedItem = equippableItem;
		}

		public void PickupItem(Item item)
		{
			item.OnPickup(Actor);

			var rb = item.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.Sleep();
				rb.detectCollisions = false;
			}

			if (EquippedItem == null && item.EquipOnPickup && item is IEquippableItem equippable)
			{
				equippable.OnEquip(Actor);
			}
			else
			{
				item.transform.SetParent(transform);
			}
			item.gameObject.SetActive(false);
			OnInventoryUpdate.Invoke(Actor, eInventoryAction.PICKUP, item);
		}

		public void DropItem(Item item)
		{
			DropItem(item, transform.position, transform.rotation);
		}

		public void DropItem(Item item, Vector3 position, Quaternion rotation)
		{
			if (item is IEquippableItem equippable && EquippedItem == equippable)
			{
				equippable.OnUnequip(Actor);
				EquippedItem = null;
			}

			item.gameObject.SetActive(true);
			item.transform.position = position;
			item.transform.rotation = rotation;
			item.transform.SetParent(null);
			item.OnDrop(Actor);

			var rb = item.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.WakeUp();
				rb.detectCollisions = true;
			}

			OnInventoryUpdate.Invoke(Actor, eInventoryAction.DROP, item);
		}
	}
}