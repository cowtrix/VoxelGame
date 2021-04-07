using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ColliderEvent : UnityEvent<Collider> { }

public class OnTriggerForwarder : MonoBehaviour
{
	public ColliderEvent TriggerEnter, TriggerExit;

	private void OnTriggerEnter(Collider other)
	{
		TriggerEnter?.Invoke(other);
	}

	private void OnTriggerExit(Collider other)
	{
		TriggerExit?.Invoke(other);
	}
}
