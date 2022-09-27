using vSplines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class Trainline : ExtendedMonoBehaviour
{
    public List<RailTower> Line;
    public TrainController Train;

    public Spline Track;
    public float Resolution = 2;

    public float DistanceTest;

    [ContextMenu("Invalidate")]
    public void Invalidate()
    {
        Track = new Spline();
        RailTower current = Line.First();
        var lengthAccum = 0f;
        foreach (var next in Line.Skip(1))
        {
            var innerSegment = new SplineSegment
            {
                FirstControlPoint = new SplineSegment.ControlPoint
                {
                    Position = current.GetLeftRailPosition(),
                    Control = current.transform.right,
                    UpVector = current.transform.up,
                },
                SecondControlPoint = new SplineSegment.ControlPoint
                {
                    Position = current.GetRightRailPosition(),
                    Control = -current.transform.right,
                    UpVector = current.transform.up,
                },
                Resolution = Resolution,
            };
            innerSegment.Recalculate();
            Track.Segments.Add(innerSegment);

            current.Distance = lengthAccum + innerSegment.Length / 2f + current.StopOffset;
            lengthAccum += innerSegment.Length;

            var connectionSegment = new SplineSegment
            {
                FirstControlPoint = new SplineSegment.ControlPoint
                {
                    Position = current.GetRightRailPosition(),
                    Control = current.transform.right * current.Curviness,
                    UpVector = current.transform.up,
                },
                SecondControlPoint = new SplineSegment.ControlPoint
                {
                    Position = next.GetLeftRailPosition(),
                    Control = -next.transform.right * current.Curviness,
                    UpVector = next.transform.up,
                },
                Resolution = Resolution,
            };
            connectionSegment.Recalculate();
            Track.Segments.Add(connectionSegment);
            lengthAccum += connectionSegment.Length;
            next.TrySetDirty();
            current = next;
        }
        var lastSegment = new SplineSegment
        {
            FirstControlPoint = new SplineSegment.ControlPoint
            {
                Position = current.GetLeftRailPosition(),
                Control = current.transform.right,
                UpVector = current.transform.up,
            },
            SecondControlPoint = new SplineSegment.ControlPoint
            {
                Position = current.GetRightRailPosition(),
                Control = -current.transform.right,
                UpVector = current.transform.up,
            },
            Resolution = Resolution,
        };
        Track.Segments.Add(lastSegment);
        current.Distance = lengthAccum + lastSegment.Length / 2f + current.StopOffset;
        Track.Recalculate();
    }

    private void Awake()
    {
        Invalidate();
    }

    private void OnDrawGizmosSelected()
    {
        if (Track != null)
        {
            Track.DrawGizmos(Color.green);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Track.GetDistancePointAlongSpline(DistanceTest), Vector3.one);
        Gizmos.color = Color.magenta;
        foreach (var stop in Line.Where(l => l.StopTime > 0))
        {
            Gizmos.DrawWireCube(stop.transform.position, Vector3.one * 5);
        }
    }

}
