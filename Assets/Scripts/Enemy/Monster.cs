using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
	public int TotalHealth => Components.Sum(x => x.Health);
	protected IEnumerable<DestroyableVoxel> Components => GetComponentsInChildren<DestroyableVoxel>();

	protected virtual void Update()
	{
		if(TotalHealth <= 0)
		{
			Destroy(gameObject);
		}
	}
}
