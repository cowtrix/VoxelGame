using System;
using System.Collections;
using UnityEngine;

public class TransformAnimationPulse : MonoBehaviour
{
	public TransformAnimationController Controller;
	public string Name;
	public float Frequency = 10;

	private void Start()
	{
		StartCoroutine(Tick());
	}

	private IEnumerator Tick()
	{
		var waiter = new WaitForSeconds(Frequency);
		while (true)
		{
			yield return waiter;
			if(!Controller || string.IsNullOrEmpty(Name))
			{
				continue;
			}
			Controller.ExpressionQueue.Enqueue(Name);
		}
	}
}
