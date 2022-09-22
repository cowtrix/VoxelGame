using Common;
using Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Voxul.Utilities;

namespace Vehicles.AI
{
    [Serializable]
    public class VehiclePathLane
    {
        public int Index;
        public Vector3 Offset;
        public bool ReverseDirection;
        public List<VehiclePathNode> Nodes = new List<VehiclePathNode>();
        public int TargetLaneIndex;
        public Vector3 Normal => Vector3.forward * (ReverseDirection ? 1 : -1);
    }

    public class ConnectorWizard : ScriptableWizard
    {
        public VehiclePathNode Target;
        public VehiclePathNode Source => Selection.activeGameObject.GetComponent<VehiclePathNode>();

        [MenuItem("Tools/Vehicle Path Connector")]
        static void CreateWizard()
        {
            DisplayWizard<ConnectorWizard>("Create Light", "Connect");
        }

        void OnWizardCreate()
        {
            Source.Lanes[1].Nodes.Add(Target);
            Target.Lanes[0].Nodes.Add(Source);
            Source.TrySetDirty();
            Target.TrySetDirty();
        }
    }

    public class VehiclePathNode : TrackedObject<VehiclePathNode>
    {
        public List<VehiclePathLane> Lanes = new List<VehiclePathLane>();

        public float Curve = 1;
        public float Radius = 1;

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
                GizmoExtensions.DrawCircle(lane.Offset, Radius, Quaternion.identity, lane.Nodes.Any() ? Color.white : Color.red, 10);
                UnityEngine.Random.InitState(lane.Index);
                GizmoExtensions.DrawCone(lane.Offset, lane.Normal, Mathf.Deg2Rad * 30f, 3, UnityEngine.Random.ColorHSV(), 4);
                foreach (var n in lane.Nodes)
                {
                    var connectedLanes = n.Lanes.Where(l => l.Index == lane.TargetLaneIndex);
                    foreach(var connectedLane in connectedLanes)
                    {
                        if (!n.gameObject.activeInHierarchy)
                        {
                            continue;
                        }
                        var seg = new SplineSegment
                        {
                            FirstControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = lane.Offset,
                                Control = -lane.Normal * Curve,
                            },
                            SecondControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = transform.worldToLocalMatrix.MultiplyPoint3x4(n.transform.localToWorldMatrix.MultiplyPoint3x4(connectedLane.Offset)),
                                Control = transform.worldToLocalMatrix.MultiplyVector(n.transform.localToWorldMatrix.MultiplyVector(connectedLane.Normal)) * n.Curve
                            },
                            Resolution = .1f
                        };
                        seg.Recalculate();
                        UnityEngine.Random.InitState(connectedLane.Index);
                        seg.DrawGizmos(Util.WithAlpha(UnityEngine.Random.ColorHSV(), .25f));
                    }
                }
                //Gizmos.DrawLine(lane.Offset, lane.Offset + lane.Normal * Curve);
            }
        }
    }

}
