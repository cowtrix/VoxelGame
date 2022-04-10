using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TriggerEvent : UnityEvent<LevelTrigger> { }

public class LevelTrigger : MonoBehaviour
{
	public enum eTriggerEventType
	{
		OnPlayerEnter,
		OnPlayerExit
	}

	[Serializable]
	public struct Event
	{
		public eTriggerEventType Type;
		public float Delay;
		public TriggerEvent Action;
	}

	public List<Event> Events = new List<Event>();

	void Trigger(eTriggerEventType type)
	{
		
		foreach(var ev in Events)
		{
			if(ev.Type != type)
			{
				continue;
			}
			if(ev.Delay > 0)
			{
				StartCoroutine(TriggerAsync(ev));
			}
			else
			{
				ev.Action.Invoke(this);
			}
		}
	}

	IEnumerator TriggerAsync(Event ev)
	{
		yield return new WaitForSeconds(ev.Delay);
		ev.Action.Invoke(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<PlayerActor>();
		if (player)
		{
			Trigger(eTriggerEventType.OnPlayerEnter);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var player = other.GetComponent<PlayerActor>();
		if (player)
		{
			Trigger(eTriggerEventType.OnPlayerExit);
		}
	}
}
