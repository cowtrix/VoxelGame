using Interaction.Items;
using Newtonsoft.Json.Linq;
using Phone;
using System;
using UnityEngine;

namespace Actors
{
	public interface IFueledActor
	{
		public float ThrusterFuel { get; }
	}

	public interface ICreditConsumerActor
	{
		public int Credits { get; }
		public bool TryPurchase(IPurchaseableItem purchaseable);
	}

	public class PlayerState : ActorState, IFueledActor
	{
		public float ThrusterEfficiency { get; private set; } = -.05f;
		public float ThrusterRecharge { get; private set; } = .1f;
		[StateMin(0)]
		public float ThrusterFuel { get; private set; } = 100;

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
			ThrusterFuel = Mathf.Clamp(ThrusterFuel, 0, 100);
			if (ThrusterFuel < 100)
			{
				var delta = ThrusterRecharge * Time.deltaTime;
				ThrusterFuel += delta;
				OnStateUpdate.Invoke(Actor, nameof(ThrusterFuel), ThrusterFuel, delta);
			}
			base.Update();
		}
	}
}