using Interaction.Items;
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
			UNEQUIP,
            REFRESH
        }

		public InventoryStateUpdateEvent OnInventoryUpdate = new InventoryStateUpdateEvent();
		public IEquippableItemComponent EquippedItem
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
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.UNEQUIP, __equippedItem.Item);
				}
				__equippedItem = value;
				if(__equippedItem != null)
				{
					__equippedItem?.OnEquip(Actor);
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.EQUIP, __equippedItem.Item);
				}
			}
		}
		private IEquippableItemComponent __equippedItem;
		public IReadOnlyCollection<Item> Inventory => GetComponentsInChildren<Item>(true);
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		[StateMinMax(0, int.MaxValue)]
		public virtual int Credits { get; protected set; } = 100;
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

			if(EquippedItem != null)
			{
				EquippedItem.OnEquippedThink(Actor);
			}
		}

		protected override void OnSaveDataLoaded()
		{
			transform.position = Position;
			transform.rotation = Rotation;
		}

		public void EquipItem(IEquippableItemComponent equippableItem)
		{
			EquippedItem = equippableItem;
		}

		public void PickupItem(IItem item, bool silent = false)
		{
			item.OnPickup(Actor);

			var rb = item.gameObject.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.Sleep();
				rb.detectCollisions = false;
			}

			if (EquippedItem == null  && item is IEquippableItemComponent equippable && equippable.EquipOnPickup)
			{
				equippable.OnEquip(Actor);
			}
			else
			{
				item.transform.SetParent(transform);
			}
			item.gameObject.SetActive(false);
			OnInventoryUpdate.Invoke(Actor, silent ? eInventoryAction.REFRESH : eInventoryAction.PICKUP, item);
		}

		public void DropItem(IItem item)
		{
			DropItem(item, transform.position, transform.rotation);
		}

		public void DropItem(IItem item, Vector3 position, Quaternion rotation)
		{
			if (item is IEquippableItemComponent equippable && EquippedItem == equippable)
			{
				equippable.OnUnequip(Actor);
				EquippedItem = null;
			}

			item.gameObject.SetActive(true);
			item.transform.position = position;
			item.transform.rotation = rotation;
			item.transform.SetParent(null);
			item.OnDrop(Actor);

			var rb = item.gameObject.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.WakeUp();
				rb.detectCollisions = true;
			}

			OnInventoryUpdate.Invoke(Actor, eInventoryAction.DROP, item);
		}
	}
}