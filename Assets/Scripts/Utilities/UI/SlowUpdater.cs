using System;
using System.Collections;
using UnityEngine;
using Voxul;

public abstract class SlowUpdater : ExtendedMonoBehaviour
{
    public float ThinkSpeed = 1;
    protected virtual void Start()
	{
        StartCoroutine(Think());
	}

    protected virtual IEnumerator Think()
	{
		while (true)
		{
			try
			{
				Tick(ThinkSpeed);
			}
			catch(Exception e)
			{
				Debug.LogException(e);
			}
            yield return new WaitForSeconds(ThinkSpeed);
		}
	}

    protected abstract void Tick(float dt);
}
