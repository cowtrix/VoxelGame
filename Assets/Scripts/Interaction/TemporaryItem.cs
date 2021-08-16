using System.Collections;
using UnityEngine;

public class TemporaryItem : Item
{
	public float MaxUseDistance = 3;
	public Transform Home;

	public override void Use(Actor actor, string action)
	{
		base.Use(actor, action);
		if (action == PICK_UP)
		{
			StartCoroutine(ThinkEquipped(actor));
		}
	}


	IEnumerator ThinkEquipped(Actor actor)
	{
		var waiter = new WaitForSeconds(1);
		while (actor.State.EquippedItem == this && Vector3.Distance(transform.position, Home.position) < MaxUseDistance)
		{
			yield return waiter;
		}
		if(actor.State.Inventory.Contains(this))
		{
			actor.DropItem(this, Home.position, Home.rotation);
		}
	}

	public override void OnDrop(Actor actor)
	{
		base.OnDrop(actor);
		transform.SetParent(Home);
	}
}
