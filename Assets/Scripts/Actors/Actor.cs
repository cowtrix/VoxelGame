using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(ActorState))]
public class Actor : ExtendedMonoBehaviour, IDialogueActor
{
	[Serializable]
	public class ActorSettings
	{
		[Header("Interaction")]
		public Vector2 InteractDistance = new Vector2(0, 10);

		[Header("Inventory")]
		public Transform EquippedItemTransform;
		public int EquippedLayer = 1;
	}
	
	public ActorSettings Settings = new ActorSettings();
	public List<Interactable> Interactables { get; private set; } = new List<Interactable>();
	public Transform InventoryContainer { get; private set; }
	public ActorState State => GetComponent<ActorState>();
	public virtual string DisplayName => name;
	public Vector3 GetDialogueOffset() => DialogueOffset;
	public Vector3 DialogueOffset;

	private void Start()
	{
		InventoryContainer = new GameObject($"{name}_Inventory").transform;
		InventoryContainer.SetParent(transform);
	}

	private void OnTriggerEnter(Collider other)
	{
		var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
		if (!interactable || Interactables.Contains(interactable))
		{
			return;
		}
		Interactables.Add(interactable);
		interactable.EnterAttention(this);
	}

	private void OnTriggerExit(Collider other)
	{
		var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
		if (!interactable)
		{
			return;
		}
		Interactables.Remove(interactable);
		interactable.ExitAttention(this);
	}

	public void PickupItem(Item item)
	{
		State.Inventory.Add(item);
		item.OnPickup(this);

		if (!State.EquippedItem)
		{
			item.gameObject.layer = Settings.EquippedLayer;
			State.EquippedItem = item;
			State.EquippedItem.transform.SetParent(Settings.EquippedItemTransform);
		}
		else
		{
			item.gameObject.layer = 3;
			item.transform.SetParent(transform);
		}

		item.transform.localPosition = item.EquippedOffset;
		item.transform.localRotation = Quaternion.Euler(item.EquippedRotation);

		State.OnInventoryUpdate.Invoke(this, Item.PICK_UP, item);
	}

	public void DropItem(Item item, Vector3 position, Quaternion rotation)
	{
		State.Inventory.Remove(item);
		if (item == State.EquippedItem)
		{
			item.OnUnequip(this);
			State.EquippedItem = null;
		}
		item.transform.position = position;
		item.transform.rotation = rotation;
		item.OnDrop(this);

		State.OnInventoryUpdate.Invoke(this, Item.DROP, item);
	}
}
