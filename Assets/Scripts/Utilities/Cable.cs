using vSplines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
	public List<SplineSegment> Spline = new List<SplineSegment>();
	public LineRenderer Renderer => GetComponent<LineRenderer>();
	public float Curviness = 1;
	private Vector3[] m_points;

	private void OnValidate()
	{
		Invalidate();
	}

	public void Invalidate()
	{
		var points = new List<Vector3>();
		Vector3 lastPoint = Vector3.one * float.PositiveInfinity;
		foreach (var segment in Spline)
		{
			segment.Recalculate();
			foreach (var point in segment.Points)
			{
				if (point.Position == lastPoint)
				{
					continue;
				}
				lastPoint = point.Position;
				points.Add(point.Position);
			}
		}

		if (!points.Any())
		{
			Renderer.positionCount = 0;
			return;
		}

		if (m_points == null || points.Count != m_points.Length)
		{
			m_points = new Vector3[points.Count];
		}
		for (int i = 0; i < points.Count; i++)
		{
			m_points[i] = points[i];
		}

		Renderer.positionCount = points.Count;
		Renderer.SetPositions(m_points);
	}
}
