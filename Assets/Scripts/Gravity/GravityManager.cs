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
		GravitySources = GravitySource.Instances.ToList();
	}

	public Vector3 GetGravityForce(Vector3 worldPos)
	{
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