using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Voxul.Utilities;

[ExecuteAlways]
public class GravityManager : Singleton<GravityManager>
{
	public List<GravitySource> GravitySources { get; private set; }

	public override void Awake()
	{
		SceneManager.sceneLoaded += RefreshGravitySources;
		base.Awake();
	}

	private void RefreshGravitySources(Scene arg0, LoadSceneMode arg1)
	{
		GravitySources = GravitySource.Instances.ToList();
	}

	public Vector3 GetGravityForce(Vector3 worldPos)
	{
		if(GravitySources == null)
		{
			GravitySources = GravitySource.Instances.ToList();
		}
		var f = Vector3.zero;
		foreach (var gravitySource in GravitySources)
		{
			var gf = gravitySource.GetGravityForce(worldPos);
			if (gf.sqrMagnitude > 0 && gravitySource.Exclusive)
			{
				return gf;
			}
			f += gf;
		}
		return f;
	}
}