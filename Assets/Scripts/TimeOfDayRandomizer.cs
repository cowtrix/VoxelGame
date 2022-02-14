using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Voxul.Utilities;

public class TimeOfDayRandomizer : SlowUpdater
{
	public GameManager GameManager { get; private set; }
	public AnimationCurve ProbabilityOverTime;
	public Vector2 ThinkSpeedRandom = new Vector2(10, 10);
	public UnityEvent Off, On;

	protected override void Tick(float dt)
	{
		var gm = GameManager.Instance;
		if (!gm)
		{
			return;
		}
		var roll = Random.value;
		var chance = ProbabilityOverTime.Evaluate(gm.CurrentTime.NormalizedTime);
		if (roll > chance)
		{
			Off.Invoke();
		}
		else
		{
			On.Invoke();
		}
		ThinkSpeed = Random.Range(ThinkSpeedRandom.x, ThinkSpeedRandom.y);
	}
}
