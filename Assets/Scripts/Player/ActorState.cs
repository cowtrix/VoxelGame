using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : StateContainer
{
	public InventoryStateUpdateEvent OnInventoryUpdate = new InventoryStateUpdateEvent();
	public Item EquippedItem
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
	private Item __equippedItem;
	public List<Item> Inventory = new List<Item>();
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	private void Awake()
	{
		Inventory = new List<Item>(GetComponentsInChildren<Item>());
	}

	protected virtual void Update()
	{
		Position = transform.position;
		Rotation = transform.rotation;
	}
}
