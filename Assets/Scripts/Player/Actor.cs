using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class Actor : ExtendedMonoBehaviour
{
	[Serializable]
	public class ActorSettings
	{
		public float MinimumDistance = 0;
		public float MaximumDistance = 10;
	}

	public ActorSettings ActorConfiguration = new ActorSettings();
	public List<Interactable> Interactables = new List<Interactable>();

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
}
