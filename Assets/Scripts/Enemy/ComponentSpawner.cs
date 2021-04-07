using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
	public GameObject SpawnObject;
	public Vector3 SpawnPosition;
	public bool ParentObject;
	public bool InheritRotation = true;
    public Vector2 SpawnInterval = Vector2.one;
    public DestroyableVoxel DestroyableVoxel;

	private void Start()
	{
		StartCoroutine(Spawn());
	}

	private IEnumerator Spawn()
	{
		while(true)
		{
			var nextWait = Mathf.Max(Random.Range(SpawnInterval.x, SpawnInterval.y), 0.1f);
			yield return new WaitForSeconds(nextWait);

			if(DestroyableVoxel && DestroyableVoxel.Health <= 0)
			{
				continue;
			}
			var newObj = Instantiate(SpawnObject);
			if(ParentObject)
			{
				newObj.transform.SetParent(transform);
			}
			var pos = transform.localToWorldMatrix.MultiplyPoint3x4(SpawnPosition);
			newObj.transform.position = pos;
			if(InheritRotation)
			{
				newObj.transform.rotation = transform.rotation;
			}
		}
	}
}
