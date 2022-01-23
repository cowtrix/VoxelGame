using Items;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : StateContainer, ICreditConsumerActor
{
	public InventoryStateUpdateEvent OnInventoryUpdate = new InventoryStateUpdateEvent();
	public IEquippableItem EquippedItem
	{
		get
		{
			return __equippedItem;
		}
		set
		{
			__equippedItem?.OnUnequip(Actor);
			__equippedItem = value;
			__equippedItem?.OnEquip(Actor);
		}
	}
	private IEquippableItem __equippedItem;
	public IReadOnlyCollection<Item> Inventory => GetComponentsInChildren<Item>();
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

	public virtual bool TryPurchase(IPurchaseable purchaseable)
	{
		var cost = purchaseable.Cost;
		return TryAdd(nameof(Credits), -cost);
	}

	public void PickupItem(Item item)
	{
		item.OnPickup(Actor);

		var rb = item.GetComponent<Rigidbody>();
		if (rb)
		{
			rb.Sleep();
			rb.interpolation = RigidbodyInterpolation.None;
		}

		if (EquippedItem == null && item.EquipOnPickup && item is IEquippableItem equippable)
		{
			equippable.OnEquip(Actor);			
		}
		else
		{
			item.gameObject.layer = 3;
			item.transform.SetParent(transform);
		}

		OnInventoryUpdate.Invoke(Actor, Item.PICK_UP, item);
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
		item.transform.position = position;
		item.transform.rotation = rotation;
		item.transform.SetParent(null);
		item.OnDrop(Actor);

		var rb = item.GetComponent<Rigidbody>();
		if (rb)
		{
			rb.WakeUp();
		}

		OnInventoryUpdate.Invoke(Actor, Item.DROP, item);
	}
}
