using Interaction.Items;
using Phone;
using System;
using UnityEngine;

namespace Actors
{
	public interface IFueledActor
	{
		public float Fuel { get; }
	}

	public interface ICreditConsumerActor
	{
		public int Credits { get; }
		public bool TryPurchase(IPurchaseableItem purchaseable, string sellerDescription);
	}

	public class PlayerState : ActorState, IFueledActor
	{
		public float ThrusterRecharge { get; private set; } = .1f;
		[StateMin(0)]
		public float Fuel { get; private set; } = 100;

		public override int Credits
		{
			get
			{
				return (Actor as PlayerActor).Phone.GetApp<PhoneAppBank>().Credits;
			}
			protected set
			{
				(Actor as PlayerActor).Phone.GetApp<PhoneAppBank>().Credits = value;
			}
		}

		protected override void Update()
		{
			Fuel = Mathf.Clamp(Fuel, 0, 100);
			if (Fuel < 100)
			{
				var delta = ThrusterRecharge * Time.deltaTime;
				Fuel += delta;
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(eStateKey.Fuel, "Jetpack", Fuel, delta, true));
			}
			base.Update();
		}
	}
}