using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class RailTower : ExtendedMonoBehaviour
{
    public RailTower NextTower;
	public Vector3 Offset;
	public float Curviness = 1;
    public BezierConnectorLineRenderer LeftOut, RightOut;
    public Transform LeftIn, RightIn;

    [ContextMenu("Invalidate")]
    public void Invalidate()
	{
		if (!NextTower)
		{
			LeftOut.Clear();
			RightOut.Clear();
			return;
		}
		LeftOut.Start.Transform = LeftOut.transform;
		LeftOut.End.Transform = NextTower.LeftIn;

		RightOut.Start.Transform = RightOut.transform;
		RightOut.End.Transform = NextTower.RightIn;

		LeftOut.Start.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(LeftOut.Start.Transform.right);
		RightOut.Start.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(RightOut.Start.Transform.right);

		LeftOut.End.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(-NextTower.LeftIn.right);
		RightOut.End.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(-NextTower.RightIn.right);

		LeftOut.Invalidate();
		RightOut.Invalidate();
	}

	public Vector3 GetRailPosition()
	{
		return transform.position + transform.localToWorldMatrix.MultiplyVector(Offset);
	}
}
