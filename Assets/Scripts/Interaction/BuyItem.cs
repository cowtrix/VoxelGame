using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyItem : MonoBehaviour
{
    public uint Cost = 10;

    public void OnInteract(PlayerController player)
	{
		var state = player.GetComponent<PlayerState>();
		if(state.CurrentState.Credits < Cost)
		{
			return;
		}
		state.CurrentState.Credits -= Cost;
	}
}
