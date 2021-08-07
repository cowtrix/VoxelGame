using UnityEngine;

public class PlayerState : ActorState
{
	public uint Credits = 100;
	public bool HeadlightOn;
	public float ThrusterFuel = 100;
	public float ThrusterEfficiency = -1;
	public float ThrusterRecharge = .1f;

	protected override void Update()
	{
		ThrusterFuel = Mathf.Clamp(ThrusterFuel, 0, 100);
		if (ThrusterFuel < 100)
		{
			ThrusterFuel += ThrusterRecharge * Time.deltaTime;
		}
		base.Update();
	}
}