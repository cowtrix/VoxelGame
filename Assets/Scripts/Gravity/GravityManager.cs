using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class GravityManager : Singleton<GravityManager>
{
	public Vector3 GetGravityForce(Vector3 worldPos)
	{
		var f = Vector3.zero;
		foreach (var gravitySource in GravitySource.Instances)
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