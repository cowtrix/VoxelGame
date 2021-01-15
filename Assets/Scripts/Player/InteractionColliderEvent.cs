using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionColliderEvent : MonoBehaviour
{
	public PlayerInteractionManager Player;
	private void OnTriggerEnter(Collider other)
	{
		var interactable = other.GetComponent<Interactable>();
		if(!interactable)
		{
			return;
		}
		interactable.OnEnterInteractionZone.Invoke(Player);
	}

	private void OnTriggerExit(Collider other)
	{
		var interactable = other.GetComponent<Interactable>();
		if (!interactable)
		{
			return;
		}
		interactable.OnExitInteractionZone.Invoke(Player);
	}
}
