using System;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
	public const string PICK_UP = "Pick Up";
	public const string DROP = "Drop";

	public Vector3 EquippedOffset, EquippedRotation;

	private int m_layer;
	private bool m_isKinematic;
	public string ItemName = "Unknown Item";

	protected Rigidbody Rigidbody => GetComponent<Rigidbody>();

	public override string DisplayName => ItemName;

	private void Start()
	{
		m_layer = gameObject.layer;
		var rb = Rigidbody;
		if (rb)
		{
			m_isKinematic = rb.isKinematic;
		}
	}

	public override IEnumerable<string> GetActions(Actor actor)
	{
		yield return PICK_UP;
	}

	public override void Use(Actor actor, string action)
	{
		if(action == PICK_UP)
		{
			actor.PickupItem(this);
		}
		base.Use(actor, action);
	}

	public virtual void OnUnequip(Actor actor)
	{

	}

	public virtual void OnEquip(Actor actor)
	{

	}

	public virtual void OnPickup(Actor actor)
	{
		var rb = Rigidbody;
		if (rb)
		{
			rb.isKinematic = true;
			rb.detectCollisions = false;
		}
	}

	public virtual void OnDrop(Actor actor)
	{
		var rb = Rigidbody;
		if (rb)
		{
			rb.position = transform.position;
			rb.rotation = transform.rotation;
			rb.isKinematic = m_isKinematic;
			rb.detectCollisions = true;
		}
		gameObject.layer = m_layer;
		transform.SetParent(null, true);
	}

	public virtual void UseOn(Actor playerInteractionManager, Interactable focusedInteractable)
	{
		
	}
}
