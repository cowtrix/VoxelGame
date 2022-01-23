using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class FuelBuddy : Interactable
{
	[Range(0, 100)]
    public float CurrentCapacity = 100;
	private float m_visibleCapacity;
	public float CostPerUnit = .25f;
	public float RechargeSpeed = .5f;
	public Transform FuelGauge;

	public override string DisplayName => "Fuel Buddy";

	public override IEnumerable<string> GetActions(Actor context)
	{
		if (!CanUse(context))
		{
			yield break;
		}
		if (GetCostForActor(context, out var refuelAmount, out var refuelCount) && refuelAmount > 0)
		{
			yield return $"Refuel {refuelAmount} Units ({refuelCount}c)";
		}
	}

	private bool GetCostForActor(Actor actor, out float refuelAmount, out int refuelCost)
	{
		if (!actor.State.TryGetValue<float>(nameof(IFueledActor.ThrusterFuel), out var currentFuel) ||
			!actor.State.TryGetValue<int>(nameof(ICreditConsumerActor.Credits), out var currentCredits))
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

	public override void Use(Actor actor, string action)
	{
		if(GetCostForActor(actor, out var refuelAmount, out var refuelCost)
			&& actor.State.TryAdd(nameof(ICreditConsumerActor.Credits), -refuelCost))
		{
			actor.State.TryAdd(nameof(IFueledActor.ThrusterFuel), refuelAmount);
			CurrentCapacity -= refuelAmount;
		}
		base.Use(actor, action);
	}

	private void Start()
	{
		m_visibleCapacity = CurrentCapacity;
	}

	private void Update()
	{
		FuelGauge.localScale = new Vector3(FuelGauge.localScale.x, Mathf.Lerp(0.1f, 1f, m_visibleCapacity / 100f), FuelGauge.localScale.z);
		m_visibleCapacity = Mathf.MoveTowards(m_visibleCapacity, CurrentCapacity, Time.deltaTime * 50);
		if(CurrentCapacity > 100)
		{
			CurrentCapacity += Time.deltaTime * RechargeSpeed;
		}
	}
}
