using Actors;
using Actors.NPC.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
	public class InteractionColliderEvent : MonoBehaviour
	{
		public PlayerActor Player;
		private void OnTriggerEnter(Collider other)
		{
			var interactable = other.GetComponent<Interactable>();
			if (!interactable)
			{
				return;
			}
			interactable.InteractionSettings.OnEnterAttention.Invoke(Player);
		}

		private void OnTriggerExit(Collider other)
		{
			var interactable = other.GetComponent<Interactable>();
			if (!interactable)
			{
				return;
			}
			interactable.InteractionSettings.OnExitAttention.Invoke(Player);
		}
	}
}