using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryUpdateEntry : UIStateUpdateEntry
{
	protected override void Awake()
	{
		base.Awake();
		PlayerState.OnInventoryUpdate.AddListener(OnInventoryUpdate);
	}

	public Text Text;

	private void OnInventoryUpdate(Actor actor, string action, Item item)
	{
		if(action == Item.PICK_UP)
		{
			Text.text = $"Picked up {item.DisplayName}".ToUpper();
		}
		else if(action == Item.DROP)
		{
			Text.text = $"Dropped {item.DisplayName}".ToUpper();
		}
		m_lastUpdate = Time.time;
		Active = true;
	}
}