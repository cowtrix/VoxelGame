using Actors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FloatEvent : UnityEvent<float> { }

[Serializable]
public class StringEvent : UnityEvent<string> { }

namespace UI
{

	public class UIStatusUpdateEntry : UIStateUpdateEntry
	{
		public string StateName;
		public FloatEvent Delta, Value;
		public StringEvent DeltaString, ValueString;

		protected override void Awake()
		{
			base.Awake();
			PlayerState.OnStateUpdate.AddListener(StateUpdate);
		}

		protected void OnDelta(Actor actor, string fieldName, float value, float delta)
		{
			Delta.Invoke(delta);
			if (delta == 0)
			{
				DeltaString.Invoke("");
			}
			else
			{
				DeltaString.Invoke($"({delta})");
			}
			Value.Invoke(value);
			ValueString.Invoke(value.ToString());
			Active = true;
		}

		protected bool ShouldActivate(Actor actor, string fieldName, float value, float delta) => fieldName == StateName;

		private void StateUpdate(Actor actor, string fieldName, float value, float delta)
		{
			m_lastUpdate = Time.time;
			if (ShouldActivate(actor, fieldName, value, delta))
			{
				OnDelta(actor, fieldName, value, delta);
			}
		}
	}
}