using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class GravityManager : Singleton<GravityManager>
{
	public Vector3 DefaultGravity = Vector3.down;
    public List<GravitySource> GravitySources;
	public Material LevelVoxelMaterial;

	private void Start()
	{
		GravitySources = FindObjectsOfType<GravitySource>().ToList();
	}

	public Vector3 GetGravityForce(Vector3 worldPos)
	{
		if(GravitySources.Count == 0)
		{
			return DefaultGravity * 1000;
		}
		var f = Vector3.zero;
		for (int i = 0; i < GravitySources.Count; i++) 
		{
			f += GravitySources[i].GetGravityForce(worldPos);
		}
		return f;
	}

	private void Update()
	{
		LevelVoxelMaterial.SetVector("Vector3_d6516d54003a46448fffec6056b370a9", GetGravityForce(CameraController.Instance.transform.position).normalized);
	}
}