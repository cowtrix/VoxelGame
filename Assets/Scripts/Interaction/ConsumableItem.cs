
using System;
using System.Collections.Generic;

[Serializable]
public class IntResourceDelta
{
	public string ResourceName;
	public int Amount;
}

public class ConsumableItem : Item
{
	public List<IntResourceDelta> Deltas;
	public bool ConsumeOnPickup;

	public override void OnPickup(Actor actor)
	{
		base.OnPickup(actor);
		if (ConsumeOnPickup)
		{
			Consume(actor);
		}
	}

	public void Consume(Actor actor)
	{
		foreach (var delta in Deltas)
		{
			actor.State.TryAdd(delta.ResourceName, delta.Amount);
		}
		Destroy(gameObject);
	}
}
