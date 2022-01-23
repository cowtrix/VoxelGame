using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Voxul.Utilities;

namespace Phone
{
	public class InventoryApp : PhoneApp
	{
		public InventoryAppItem AppItemPrefab;
		public RectTransform ItemContainer, DetailsTransform;
		public Text DetailsText;
		public Button DropButton, EquipButton, ConsumeButton;
		public ToggleGroup ToggleGroup => ItemContainer.GetComponent<ToggleGroup>();
		public Item FocusedItem { get; private set; }

		private List<InventoryAppItem> m_items = new List<InventoryAppItem>();

		public override void Initialise(PhoneController controller)
		{
			base.Initialise(controller);
			controller.Actor.State.OnInventoryUpdate.AddListener(OnItemPickup);
			Invalidate();
		}

		private void OnItemPickup(Actor actor, string action, Item item)
		{
			Phone.NotificationManager.CreateNotification(this, GetActionString(action, item));
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
		}

		private void Update()
		{
			var targetScale = Vector3.one;
			if (!FocusedItem)
			{
				targetScale = targetScale.Flatten();
			}
			DetailsTransform.localScale = Vector3.MoveTowards(DetailsTransform.localScale, targetScale, Time.deltaTime * 5);

			ConsumeButton.gameObject.SetActive(FocusedItem is IConsumableItem);
			EquipButton.gameObject.SetActive(FocusedItem is IEquippableItem);
		}

		private string GetActionString(string action, Item item)
		{
			switch (action)
			{
				case Item.DROP:
					return $"Dropped {item.DisplayName}";
				case Item.PICK_UP:
					return $"Picked up {item.DisplayName}";
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
			if (!FocusedItem || !(FocusedItem is IConsumableItem consumable))
			{
				return;
			}
			consumable.Consume(Phone.Actor);
			Invalidate();
		}

		public void Equip()
		{
			if(!FocusedItem || !(FocusedItem is IEquippableItem equippable))
			{
				return;
			}
			equippable.OnEquip(Phone.Actor);
			Invalidate();
		}
	}
}