using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class RailTower : ExtendedMonoBehaviour
{
    public RailTower NextTower;

	public float StopTime = 0;

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
		LeftOut.StartTransform.Transform = LeftOut.transform;
		LeftOut.EndTransform.Transform = NextTower.LeftIn;

		RightOut.StartTransform.Transform = RightOut.transform;
		RightOut.EndTransform.Transform = NextTower.RightIn;

		LeftOut.EndTransform.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(-LeftOut.StartTransform.Transform.right);
		RightOut.EndTransform.Normal = Curviness * transform.worldToLocalMatrix.MultiplyVector(-RightOut.StartTransform.Transform.right);

		LeftOut.StartTransform.Normal = Curviness * Vector3.right;
		RightOut.StartTransform.Normal = Curviness * Vector3.right;

		LeftOut.Invalidate();
		RightOut.Invalidate();
	}

	public Vector3 GetRailPosition()
	{
		return transform.position + transform.localToWorldMatrix.MultiplyVector(Offset);
	}
}
