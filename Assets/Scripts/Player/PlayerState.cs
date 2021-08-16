using UnityEngine;

public interface IFueledActor
{
	public float ThrusterFuel { get; }
}

public interface ICreditedActor
{
	public int Credits { get; }
}

public class PlayerState : ActorState, IFueledActor, ICreditedActor
{
	public float ThrusterEfficiency { get; private set; } = -.05f;
	public float ThrusterRecharge { get; private set; } = .1f;
	[StateMin(0)]
	public float ThrusterFuel { get; private set; } = 100;
	[StateMin(0)]
	public int Credits { get; private set; } = 150;

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