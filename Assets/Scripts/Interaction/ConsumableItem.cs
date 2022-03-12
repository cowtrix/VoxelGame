
using Actors;
using System;
using System.Collections.Generic;

namespace Interaction.Items
{
	[Serializable]
	public class IntResourceDelta
	{
		public eStateKey ResourceKey;
		public int Amount;
		public string Description;
	}

	public interface IConsumableItem
	{
		void Consume(Actor actor);
	}

	public class ConsumableItem : Item, IConsumableItem
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
				actor.State.TryAdd(delta.ResourceKey, delta.Amount, delta.Description);
			}
			Destroy(gameObject);
		}
	}
}