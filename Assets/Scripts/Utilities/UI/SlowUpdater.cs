using System.Collections;
using UnityEngine;
using Voxul;

public abstract class SlowUpdater : ExtendedMonoBehaviour
{
    public float ThinkSpeed = 1;
    protected virtual void OnEnable()
	{
        StartCoroutine(Think());
	}

    protected virtual IEnumerator Think()
	{
		while (true)
		{
            Tick(ThinkSpeed);
            yield return new WaitForSeconds(ThinkSpeed);
		}
	}

    protected abstract void Tick(float dt);
}
