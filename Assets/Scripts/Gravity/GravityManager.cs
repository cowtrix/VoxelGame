using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class GravityManager : Singleton<GravityManager>
{
    public List<GravitySource> GravitySources { get; private set; }

	private void Start()
	{
		GravitySources = FindObjectsOfType<GravitySource>().ToList();
	}

	public Vector3 GetGravityForce(Vector3 worldPos)
	{
		if(GravitySources.Count == 0)
		{
			return Vector3.zero;
		}
		var f = Vector3.zero;
		for (int i = 0; i < GravitySources.Count; i++) 
		{
			var gs = GravitySources[i];
			var gf = gs.GetGravityForce(worldPos);
			if (gf.sqrMagnitude > 0 && gs.Exclusive)
			{
				return gf;
			}
			f += gf;
		}
		return f;
	}
}