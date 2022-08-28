using Actors;
using Common;
using Interaction.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voxul.Utilities;

namespace Phone
{
	public class InventoryApp : PhoneApp
	{
		public TextMeshProUGUI EmptyText;
		public TextMeshProUGUI DetailsText;
		public InventoryAppItem AppItemPrefab;
		public RectTransform ItemContainer, DetailsTransform;
		public GameObject DropButton, EquipButton, ConsumeButton;
		public ToggleGroup ToggleGroup => ItemContainer.GetComponent<ToggleGroup>();
		public Item FocusedItem { get; private set; }

		private List<InventoryAppItem> m_items = new List<InventoryAppItem>();

		public override void Initialise(PhoneController controller)
		{
			base.Initialise(controller);
			controller.Actor.State.OnInventoryUpdate.AddListener(OnInventoryUpdate);
			Invalidate();
		}

		private void OnInventoryUpdate(Actor actor, ActorState.eInventoryAction action, IItem item)
		{
			if(action == ActorState.eInventoryAction.PICKUP)
            {
				Phone.NotificationManager.CreateNotification(this, GetActionString(action, item));
			}
			Invalidate();
		}

		private void Invalidate()
		{
			var items = Phone.Actor.State.Inventory.GroupBy(i => i.DisplayName);
			int counter = 0;
			foreach (var g in items)
			{
				var count = g.Count();
				var item = g.First();
				InventoryAppItem itemDisplay = null;
				if (counter >= m_items.Count)
				{
					itemDisplay = Instantiate(AppItemPrefab.gameObject).GetComponent<InventoryAppItem>();
					itemDisplay.transform.SetParent(ItemContainer);
					itemDisplay.transform.Reset();
					m_items.Add(itemDisplay);
				}
				else
				{
					itemDisplay = m_items[counter];
				}
				itemDisplay.Toggle.onValueChanged.RemoveAllListeners();
				itemDisplay.Toggle.isOn = false;
				itemDisplay.Toggle.group = ToggleGroup;
				itemDisplay.Toggle.onValueChanged.AddListener((b) => { if (b) { FocusItem(item); } });
				itemDisplay.SetData(item, count);
				counter++;
			}
			for (var i = m_items.Count - 1; i >= counter; --i)
			{
				m_items[i].gameObject.SafeDestroy();
				m_items.RemoveAt(i);
			}
			EmptyText.gameObject.SetActive(counter == 0);
		}

		private void Update()
		{
			if (!Phone.Actor.State.Inventory.Contains(FocusedItem))
			{
				FocusedItem = null;
			}
			ConsumeButton.SetActive(FocusedItem && FocusedItem.Implements<ConsumableItemComponent>());
			EquipButton.SetActive(FocusedItem && FocusedItem.Implements<IEquippableItemComponent>());
			var targetScale = Vector3.one;
			if (!FocusedItem)
			{
				targetScale = targetScale.Flatten();
			}
			DetailsTransform.localScale = Vector3.MoveTowards(DetailsTransform.localScale, targetScale, Time.deltaTime * 5);
		}

		private string GetActionString(ActorState.eInventoryAction action, IItem item)
		{
			switch (action)
			{
				case ActorState.eInventoryAction.DROP:
					return $"Dropped {item.DisplayName}";
				case ActorState.eInventoryAction.PICKUP:
					return $"Picked up {item.DisplayName}";
				case ActorState.eInventoryAction.EQUIP:
					return $"Equipped {item.DisplayName}";
				case ActorState.eInventoryAction.UNEQUIP:
					return $"Unequipped {item.DisplayName}";
				default:
					return $"Did a thing to {item.DisplayName}";
			}
		}

		private void FocusItem(Item item)
		{
			FocusedItem = item;
			DetailsText.text = $"<b>{item.DisplayName}</b> {item.Description}";
		}

		public void Drop()
		{
			if (!FocusedItem)
			{
				return;
			}
			Phone.Actor.State.DropItem(FocusedItem);
			Invalidate();
		}

		public void Consume()
		{
			if (!FocusedItem)
			{
				return;
			}
			var consumable = FocusedItem.GetComponent<ConsumableItemComponent>();
            if (!consumable)
            {
				return;
            }
			consumable.Consume(Phone.Actor);
			Invalidate();
		}

		public void Equip()
		{
			if (!FocusedItem)
			{
				return;
			}
			var equippable = FocusedItem.GetComponentByInterface<IEquippableItemComponent>();
			if(equippable == null || equippable.Equals(null))
            {
				return;
            }
			Phone.Actor.State.EquipItem(equippable);
			Invalidate();
		}
	}
}