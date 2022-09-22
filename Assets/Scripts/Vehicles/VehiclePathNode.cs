using Common;
using Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vehicles.AI
{
    [Serializable]
    public class VehiclePathLane
    {
        public Vector3 Offset;
        public bool ReverseDirection;
        public VehiclePathNode Node;
        public int TargetLaneIndex;
        public Vector3 Normal => Vector3.forward * (ReverseDirection ? 1 : -1);
    }

    public class VehiclePathNode : TrackedObject<VehiclePathNode>
    {
        public List<VehiclePathLane> Lanes = new List<VehiclePathLane>();

        public float Curve = 1;
        public float Radius = 1;
        public float LaneSnapDistance = 100;

        public Vector3 GetWorldLanePosition(int laneIndex)
        {
            return transform.localToWorldMatrix.MultiplyPoint3x4(Lanes[laneIndex].Offset);
        }

        public Vector3 GetWorldNormal(int laneIndex)
        {
            return transform.localToWorldMatrix.MultiplyVector(Lanes[laneIndex].Normal) * Curve;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            for (int i = 0; i < Lanes.Count; i++)
            {
                VehiclePathLane lane = Lanes[i];
                GizmoExtensions.DrawCircle(lane.Offset, Radius, Quaternion.identity, Color.white);
                GizmoExtensions.DrawCone(lane.Offset, lane.Normal, Mathf.Deg2Rad * 30f, 3, Color.white, 4);
                if (lane.Node)
                {
                    var connectedLane = lane.Node.Lanes[lane.TargetLaneIndex];
                    var seg = new SplineSegment
                    {
                        FirstControlPoint = new SplineSegment.ControlPoint
                        {
                            Position = lane.Offset,
                            Control = -lane.Normal * Curve,
                        },
                        SecondControlPoint = new SplineSegment.ControlPoint
                        {
                            Position = transform.worldToLocalMatrix.MultiplyPoint3x4(lane.Node.transform.localToWorldMatrix.MultiplyPoint3x4(connectedLane.Offset)),
                            Control = transform.worldToLocalMatrix.MultiplyVector(lane.Node.transform.localToWorldMatrix.MultiplyVector(connectedLane.Normal))
                        },
                        Resolution = .3f
                    };
                    seg.Recalculate();
                    seg.DrawGizmos(Color.white);
                }
                Gizmos.DrawLine(lane.Offset, lane.Offset + lane.Normal * Curve);
            }
        }
    }

}
