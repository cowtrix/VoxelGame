using Actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction
{
	public class FuelBuddy : Interactable
	{
		[Range(0, 100)]
		public float CurrentCapacity = 100;
		private float m_visibleCapacity;
		public float CostPerUnit = .25f;
		public float RechargeSpeed = .5f;
		public Transform FuelGauge;

		public override string DisplayName => "Fuel Buddy";

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (!CanUse(context))
			{
				yield break;
			}
			if (GetCostForActor(context, out var refuelAmount, out var refuelCount) && refuelAmount > 0)
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = $"Refuel {refuelAmount} Units ({refuelCount}c)" };
			}
		}

		private bool GetCostForActor(Actor actor, out float refuelAmount, out int refuelCost)
		{
			if (!actor.State.TryGetValue<float>(eStateKey.Fuel, out var currentFuel) ||
				!actor.State.TryGetValue<int>(eStateKey.Credits, out var currentCredits))
			{
				refuelAmount = 0;
				refuelCost = 0;
				return false;
			}
			refuelAmount = Mathf.Floor(Mathf.Min(100 - currentFuel, CurrentCapacity));
			refuelCost = Mathf.RoundToInt(CostPerUnit * refuelAmount);
			if (currentCredits < refuelCost)
			{
				refuelAmount = Mathf.Floor(Mathf.Min(currentCredits / CostPerUnit, CurrentCapacity));
				refuelCost = Mathf.RoundToInt(CostPerUnit * refuelAmount);
			}
			return true;
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
		{
			if(action.Key == eActionKey.USE)
			{
				if (GetCostForActor(actor, out var refuelAmount, out var refuelCost)
				&& actor.State.TryAdd(eStateKey.Credits, -refuelCost, DisplayName))
				{
					actor.State.TryAdd(eStateKey.Fuel, refuelAmount, DisplayName);
					CurrentCapacity -= refuelAmount;
				}
			}
			base.ReceiveAction(actor, action);
		}

		protected override void Start()
		{
			m_visibleCapacity = CurrentCapacity;
		}

		private void Update()
		{
			FuelGauge.localScale = new Vector3(FuelGauge.localScale.x, Mathf.Lerp(0.1f, 1f, m_visibleCapacity / 100f), FuelGauge.localScale.z);
			m_visibleCapacity = Mathf.MoveTowards(m_visibleCapacity, CurrentCapacity, Time.deltaTime * 50);
			if (CurrentCapacity > 100)
			{
				CurrentCapacity += Time.deltaTime * RechargeSpeed;
			}
		}
	}
}