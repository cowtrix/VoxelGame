using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Voxul.Utilities;

public class TimeOfDayRandomizer : SlowUpdater
{
	public GameManager GameManager => GameManager.Instance;
	public AnimationCurve ProbabilityOverTime;
	public Vector2 ThinkSpeedRandom = new Vector2(10, 10);
	public UnityEvent Off, On;

	private bool? m_lastValue;
	private float m_timer;

	protected override int TickOnThread(float dt)
	{
		m_timer -= dt;
		if(m_timer > 0)
        {
			return 0;
        }
		var roll = Random.value;
		var success = roll < ProbabilityOverTime.Evaluate(GameManager.CurrentTime.NormalizedTime);
		if(m_lastValue.HasValue && success == m_lastValue)
        {
			return 0;
        }
		m_lastValue = success;
		if (!success)
		{
			Off.Invoke();
		}
		else
		{
			On.Invoke();
		}
		m_timer = Random.Range(ThinkSpeedRandom.x, ThinkSpeedRandom.y);
		return 1;
	}
}
