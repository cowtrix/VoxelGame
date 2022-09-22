using Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

public class Trainline : ExtendedMonoBehaviour
{
    public List<RailTower> Line;
	public TrainController Train;
	[SerializeField]
	private List<SplineSegment> Splines;
	[SerializeField]
	[HideInInspector]
	public float TotalDistance;
	public float Resolution = 2;

	[ContextMenu("Invalidate")]
	public void Invalidate()
	{
		TotalDistance = 0;
		Splines.Clear();
		RailTower current = Line.First();
		foreach(var next in Line.OrderBy(r => r.transform.position.y).Skip(1))
		{
			var segmnent = new SplineSegment
			{
				Resolution = Resolution,
				FirstControlPoint = new SplineSegment.ControlPoint
				{
					Position = current.GetRailPosition(),
					Control = current.transform.right * current.Curviness,
					UpVector = current.transform.up,
				},

				SecondControlPoint = new SplineSegment.ControlPoint
				{
					Position = next.GetRailPosition(),
					Control = -next.transform.right * current.Curviness,
					UpVector = next.transform.up,
				}
			};
			segmnent.Recalculate();
			TotalDistance += segmnent.Length;
			Splines.Add(segmnent);
			current = next;
		}
	}

	public Vector3 GetPointOnLine(float time)
	{
		var targetDistance = time * TotalDistance;
		var currentDistance = 0f;
		foreach(var n in Splines)
		{
			if(currentDistance + n.Length >= targetDistance)
			{
				var adjustedTime = (targetDistance - currentDistance) / n.Length;
				var position = n.GetUniformPointOnSplineSegment(adjustedTime);
				return position;
			}	
			currentDistance += n.Length;
		}
		var last = Splines.Last();
		return last.SecondControlPoint.Position;
	}

	private void OnDrawGizmosSelected()
	{
		if(Splines == null || !Splines.Any())
		{
			return;
		}
		var lastPoint = Splines.First().FirstControlPoint.Position;
		foreach(var s in Splines)
		{
			for(var i = 0f; i < 1; i += .2f)
			{
				var p = s.GetNaturalPointOnSplineSegment(i);
				Gizmos.DrawLine(lastPoint, p);
				lastPoint = p;
			}
		}
	}

}
