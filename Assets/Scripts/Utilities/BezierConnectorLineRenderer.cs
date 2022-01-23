using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(LineRenderer))]
public class BezierConnectorLineRenderer : ExtendedMonoBehaviour
{
	[Serializable]
	public struct TransformAnchor
	{
		public Transform Transform;
		public Vector3 Offset;
		public Vector3 Normal;

		public void ApplyToSplinePoint(Transform parent, SplineSegment.ControlPoint point)
		{
			point.Position = parent.worldToLocalMatrix.MultiplyPoint(Transform.localToWorldMatrix.MultiplyPoint(Offset));
			point.Control = parent.worldToLocalMatrix.MultiplyVector(Transform.localToWorldMatrix.MultiplyVector(Normal));
			point.UpVector = Transform.up;
		}
	}

	public TransformAnchor Start = new TransformAnchor(), End = new TransformAnchor();
	public float Resolution = 10;
	private LineRenderer Renderer => GetComponent<LineRenderer>();

	[HideInInspector]
	public SplineSegment Spline;
	public void Clear()
	{
		Renderer.enabled = false;
	}

	[ContextMenu("Invalidate")]
	public void Invalidate()
	{
		if(!Start.Transform || !End.Transform)
		{
			Clear();
			return;
		}
		Renderer.enabled = true;
		if (Spline == null)
		{
			Spline = new SplineSegment();
		}
		Renderer.useWorldSpace = false;
		Start.ApplyToSplinePoint(transform, Spline.FirstControlPoint);
		End.ApplyToSplinePoint(transform, Spline.SecondControlPoint);

		Spline.Resolution = Resolution;
		Spline.Recalculate();

		Renderer.positionCount = Spline.Points.Count;
		Renderer.SetPositions(Spline.Points.Select(p => p.Position).ToArray());
	}
}
