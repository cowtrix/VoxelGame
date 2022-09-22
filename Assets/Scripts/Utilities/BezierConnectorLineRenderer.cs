using Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
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

		public Vector3 GetWorldPosition()
		{
			if (!Transform)
			{
				return Vector3.zero;
			}
			return Transform.localToWorldMatrix.MultiplyPoint3x4(Offset);
		}

		public void ApplyToSplinePoint(Transform parent, SplineSegment.ControlPoint point)
		{
			point.Position = parent.worldToLocalMatrix.MultiplyPoint(Transform.localToWorldMatrix.MultiplyPoint(Offset));
			point.Control = parent.worldToLocalMatrix.MultiplyVector(Transform.localToWorldMatrix.MultiplyVector(Normal));
			point.UpVector = Transform.up;
		}
	}

	[FormerlySerializedAs("Start")]
	public TransformAnchor StartTransform = new TransformAnchor();
	[FormerlySerializedAs("End")]
	public TransformAnchor EndTransform = new TransformAnchor();
	public TransformAnchor MidPoint;
	public float Resolution = 10;
	public float Tolerance = .01f;
	public float Curviness = 1f;
	private LineRenderer Renderer => GetComponent<LineRenderer>();

	public bool Dynamic = false;
	public int DynamicChainCount = 3;
	public Vector3 DynamicChainSize = new Vector3(.1f, .1f, .1f);
	private Vector3 m_lastStartPosition, m_lastEndPosition, m_lastMidPosition;

	[HideInInspector]
	public SplineSegment Spline;
	[HideInInspector]
	public SplineSegment Spline2;
	public void Clear()
	{
		Renderer.enabled = false;
	}

	private void Start()
	{
		m_lastStartPosition = StartTransform.GetWorldPosition();
		m_lastEndPosition = EndTransform.GetWorldPosition();
		if (Dynamic && EndTransform.Transform && StartTransform.Transform)
		{
			/*Spline.Recalculate();
			var firstRB = StartTransform.Transform.GetComponent<Rigidbody>();
			var firstOffset = StartTransform.Offset;

			Rigidbody nextRigidbody;
			Vector3 nextOffset;

			for (int i = 0; i < DynamicChainCount; i++)
			{
				var point = Spline.GetUniformPointOnSpline((i + 1) / (float)(DynamicChainCount));
				var springJoint = firstRB.gameObject.AddComponent<SpringJoint>();
				springJoint.anchor = firstOffset;
				springJoint.breakForce = 9999999;
				springJoint.autoConfigureConnectedAnchor = false;
				//springJoint.tolerance = 1;
				//springJoint.spring = 1;
				if (i < DynamicChainCount - 1)
				{
					var chainGO = new GameObject($"chain_{i}");
					chainGO.transform.SetParent(transform);
					chainGO.transform.localPosition = point;

					var collider = chainGO.AddComponent<BoxCollider>();
					collider.size = DynamicChainSize;
					//chainGO.AddComponent<GravityRigidbody>();
					nextRigidbody = chainGO.AddComponent<Rigidbody>();
					nextRigidbody.position = chainGO.transform.position;
					nextRigidbody.mass = .001f;

					nextOffset = nextRigidbody.transform.worldToLocalMatrix.MultiplyPoint3x4(collider.ClosestPoint(nextRigidbody.position));
					firstOffset = firstRB.transform.worldToLocalMatrix.MultiplyPoint3x4(collider.ClosestPoint(firstRB.position));
				}
				else
				{
					nextRigidbody = EndTransform.Transform.GetComponent<Rigidbody>();
					nextOffset = EndTransform.Offset;
					firstOffset = default;
				}
				springJoint.minDistance = Vector3.Distance(firstRB.position, nextRigidbody.position) * 1.01f;
				springJoint.connectedBody = nextRigidbody;
				springJoint.connectedAnchor = nextOffset;
				firstRB = springJoint.connectedBody;
			}*/
		}
	}

	private void Update()
	{
		if (!Dynamic)
		{
			return;
		}
		if ((m_lastStartPosition - StartTransform.GetWorldPosition()).magnitude > Tolerance
			|| (m_lastEndPosition - EndTransform.GetWorldPosition()).magnitude > Tolerance
			|| (MidPoint.Transform && (m_lastMidPosition - MidPoint.GetWorldPosition()).magnitude > Tolerance))
		{
			Invalidate();
		}
		m_lastStartPosition = StartTransform.GetWorldPosition();
		m_lastEndPosition = EndTransform.GetWorldPosition();
	}

	[ContextMenu("Invalidate")]
	public void Invalidate()
	{
		if (!StartTransform.Transform || !StartTransform.Transform.gameObject.activeInHierarchy
			|| !EndTransform.Transform || !EndTransform.Transform.gameObject.activeInHierarchy)
		{
			Clear();
			return;
		}
		Renderer.enabled = true;
		if (Spline == null)
		{
			Spline = new SplineSegment();
		}
		if (Spline2 == null)
		{
			Spline2 = new SplineSegment()
;
		}

		Renderer.useWorldSpace = false;
		StartTransform.ApplyToSplinePoint(transform, Spline.FirstControlPoint);

		if (MidPoint.Transform)
		{
			MidPoint.ApplyToSplinePoint(transform, Spline.SecondControlPoint);
			MidPoint.ApplyToSplinePoint(transform, Spline2.FirstControlPoint);
			EndTransform.ApplyToSplinePoint(transform, Spline2.SecondControlPoint);

			Spline.SecondControlPoint.Control = transform.worldToLocalMatrix.MultiplyVector(MidPoint.Transform.right) * Curviness;
			Debug.DrawLine(MidPoint.Transform.position, MidPoint.Transform.position + MidPoint.Transform.right);
			Spline2.FirstControlPoint.Control = transform.worldToLocalMatrix.MultiplyVector(-MidPoint.Transform.right) * Curviness; 
			Debug.DrawLine(MidPoint.Transform.position, MidPoint.Transform.position - MidPoint.Transform.right);

			Spline.Resolution = Resolution;
			Spline2.Resolution = Resolution;
			Spline.Recalculate();

			Renderer.positionCount = Spline.Points.Count + Spline2.Points.Count;
			var points = Spline.Points.Select(p => p.Position).ToList();
			points.AddRange(Spline2.Points.Select(p => p.Position));

			Renderer.SetPositions(points.ToArray());
		}
		else
		{
			EndTransform.ApplyToSplinePoint(transform, Spline.SecondControlPoint);

			Spline.Resolution = Resolution;
			Spline.Recalculate();

			Renderer.positionCount = Spline.Points.Count;
			Renderer.SetPositions(Spline.Points.Select(p => p.Position).ToArray());
		}


		Spline2.Resolution = Resolution;
		Spline2.Recalculate();


	}
}
