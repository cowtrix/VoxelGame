using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public abstract class PeripheralInteraction : MonoBehaviour
{
	protected virtual Interactable Interactable => GetComponent<Interactable>();

	protected virtual void Awake()
	{
		Interactable.InteractionSettings.OnEnterAttention.AddListener(p =>
		{
			//p.AddSecondaryInteractable(Interactable, () => transform.position);
		});
		Interactable.InteractionSettings.OnExitAttention.AddListener(p =>
		{
			//p.RemoveSecondaryInteractable(Interactable);
		});
	}
}
