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
	public float TargetTime { get; private set; } = 1;
	public float Floor = .3f;
	public float StopTime = 30;
	[SerializeField]
	private List<SplineSegment> Splines;
	[SerializeField]
	private float m_totalDistance;

	public float Resolution = 2;

	[ContextMenu("Invalidate")]
	public void Invalidate()
	{
		m_totalDistance = 0;
		Splines.Clear();
		RailTower current = Line.First();
		foreach(var next in Line.Skip(1))
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
			m_totalDistance += segmnent.Length;
			Splines.Add(segmnent);
			current = next;
		}
	}

	public Vector3 GetPointOnLine(float time)
	{
		var targetDistance = time * m_totalDistance;
		var currentDistance = 0f;
		foreach(var n in Splines)
		{
			if(currentDistance + n.Length >= targetDistance)
			{
				var adjustedTime = (targetDistance - currentDistance) / n.Length;
				var position = n.GetUniformPointOnSpline(adjustedTime);
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
				var p = s.GetNaturalPointOnSpline(i);
				Gizmos.DrawLine(lastPoint, p);
				lastPoint = p;
			}
		}
	}

	private void Start()
	{
		StartCoroutine(Think());
	}

	private IEnumerator Think()
	{
		while (true)
		{
			yield return null;
			if (Train.Time >= 1 - Floor)
			{
				TargetTime = Floor;
				yield return new WaitForSeconds(StopTime);
			}
			else if(Train.Time <= Floor)
			{
				TargetTime = 1;
				yield return new WaitForSeconds(StopTime);
			}
			Train.Time = Mathf.Clamp01(Mathf.MoveTowards(Train.Time, TargetTime, Time.deltaTime * Train.Speed));
		}		
	}
}
