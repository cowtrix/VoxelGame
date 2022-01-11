using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPulse : MonoBehaviour
{
    public float Delay = 0;
    public float OffTime = 1;
	public float OnTime = 1;
	public UnityEvent OnPulse, OnPulseEnd;

	private void Start()
	{
		StartCoroutine(Pulse());
	}

	private IEnumerator Pulse()
	{
		yield return new WaitForSeconds(Delay);
		while (true)
		{
			OnPulse.Invoke();
			yield return new WaitForSeconds(OnTime);
			OnPulseEnd.Invoke();
			yield return new WaitForSeconds(OffTime);
		}
	}
}
