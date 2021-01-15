using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyItem : Label
{
    public uint Cost = 10;

	public override string GetText()
	{
		return $"Buy {m_text} [{Cost}c]";
	}

	public void OnInteract(PlayerInteractionManager player)
	{
		var state = player.GetComponent<PlayerState>();
		if(state.CurrentState.Credits < Cost)
		{
			return;
		}
		state.CurrentState.Credits -= Cost;
	}
}
