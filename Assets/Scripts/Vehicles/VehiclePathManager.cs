using Splines;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

namespace Vehicles.AI
{
    public class VehiclePathManager : Singleton<VehiclePathManager>
    {
        public float PathResolution = .2f;

        private class Node
        {
            public VehiclePathNode Component { get; private set; }
            public int TargetLaneIndex => Component.Lanes[SourceLaneIndex].TargetLaneIndex;
            public int SourceLaneIndex { get; private set; }
            public Vector3 Position { get; private set; }
            public Node Parent { get; set; }
            public List<Node> Connections { get; private set; } = new List<Node>();
            public float AccumulatedCost { get; set; }
            public float DistanceToTarget { get; set; }
            public float TotalScore => AccumulatedCost + DistanceToTarget;
            public Node(VehiclePathNode node, int laneIndex)
            {
                Component = node;
                SourceLaneIndex = laneIndex;
                Position = Component.transform.position + Component.transform.localToWorldMatrix.MultiplyVector(Component.Lanes[laneIndex].Offset);
            }
        }

        public static VehiclePathNode GetClosestNode(Vector3 position, Vector3 velocity, out int laneIndex)
        {
            foreach (var node in VehiclePathNode.Instances
                .OrderBy(n => Vector3.Distance(position, n.transform.position)))
            {
                var bestLane = node.Lanes.OrderBy(l =>
                    (Vector3.Angle(velocity.normalized, node.transform.localToWorldMatrix.MultiplyVector(l.Normal.normalized))))
                    .First();
                laneIndex = bestLane.Index;
                return node;
            }
            laneIndex = -1;
            return null;
        }

        public bool GetPath(Vector3 startingPos, Vector3 startingVel, Vector3 endPos, out Spline s, float laneJoinDistance = float.MaxValue)
        {
            s = new Spline();
            var isValid = true;
            var closestNodeToStart = GetClosestNode(startingPos, startingVel, out var laneIndex);
            var closestNodeToEnd = GetClosestNode(endPos, -startingVel, out _);

            DebugHelper.DrawCube(closestNodeToStart.transform.position, Vector3.one, Quaternion.identity, Color.yellow, 0);
            DebugHelper.DrawCube(closestNodeToEnd.transform.position, Vector3.one, Quaternion.identity, Color.red, 0);

            if (closestNodeToStart != closestNodeToEnd && Vector3.Distance(startingPos, closestNodeToStart.transform.position) < laneJoinDistance)
            {
                s.Segments.Add(new SplineSegment
                {
                    FirstControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = startingPos,
                        Control = startingVel,
                    },
                    SecondControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = closestNodeToStart.GetWorldLanePosition(laneIndex),
                        Control = closestNodeToStart.GetWorldNormal(laneIndex),
                    },
                    Resolution = PathResolution,
                });

                var open = new List<Node>();
                var closed = new List<Node>();
                {
                    var n = new Node(closestNodeToStart, laneIndex);
                    n.DistanceToTarget = Vector3.Distance(n.Position, closestNodeToEnd.transform.position);
                    open.Add(n);
                }

                Node lastNode = null;
                while (open.Any())
                {
                    var next = open.OrderBy(n => n.TotalScore).First();
                    open.Remove(next);

                    DebugHelper.DrawCube(next.Position, Vector3.one * 2, Quaternion.identity, Color.blue, 0);

                    closed.Add(next);

                    if (next.Component == closestNodeToEnd)
                    {
                        lastNode = next;
                        break;
                    }

                    var lanes = next.Component.Lanes.Where(l => l.Index == next.SourceLaneIndex);
                    foreach (var lane in lanes)
                    {
                        foreach (var connectingNode in lane.Nodes)
                        {
                            if (!connectingNode.gameObject.activeInHierarchy || closed.Any(n => n.Component == connectingNode))
                            {
                                continue;
                            }
                            var newNode = new Node(connectingNode, lane.TargetLaneIndex) { Parent = next };
                            newNode.DistanceToTarget = Vector3.Distance(newNode.Position, closestNodeToEnd.transform.position);
                            if (newNode.Parent != null)
                            {
                                newNode.AccumulatedCost = newNode.Parent.AccumulatedCost
                                    + Vector3.Distance(next.Component.transform.position, newNode.Component.transform.position);
                            }
                            next.Connections.Add(newNode);

                            var existingInOpen = open.FirstOrDefault(n => n.Component == newNode.Component);
                            if (existingInOpen != null && existingInOpen.TotalScore < newNode.TotalScore)
                            {
                                continue;
                            }

                            var existingInClosed = closed.FirstOrDefault(n => n.Component == newNode.Component);
                            if (existingInClosed != null && existingInClosed.TotalScore < newNode.TotalScore)
                            {
                                continue;
                            }

                            open.Add(newNode);
                        }
                    }
                    lastNode = next;
                }

                if(lastNode.Component != closestNodeToEnd)
                {
                    isValid = false;
                }

                var path = ReconstructPath(lastNode)
                    .Reverse()
                    .ToList();

                Node lastVehicleNode = null;
                for (int i = 0; i < path.Count; i++)
                {
                    var n = path[i];
                    if (i > 0)
                    {
                        s.Segments.Add(new SplineSegment
                        {
                            FirstControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = lastVehicleNode.Component.GetWorldLanePosition(lastVehicleNode.SourceLaneIndex),
                                Control = -lastVehicleNode.Component.GetWorldNormal(lastVehicleNode.SourceLaneIndex),
                            },
                            SecondControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = n.Component.GetWorldLanePosition(n.SourceLaneIndex),
                                Control = n.Component.GetWorldNormal(n.SourceLaneIndex),
                            },
                            Resolution = PathResolution,
                        });
                    }
                    lastVehicleNode = n;
                }

                s.Segments.Add(new SplineSegment
                {
                    FirstControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = lastVehicleNode.Component.GetWorldLanePosition(lastVehicleNode.SourceLaneIndex),
                        Control = -lastVehicleNode.Component.GetWorldNormal(lastVehicleNode.SourceLaneIndex),
                    },
                    SecondControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = endPos,
                    },
                    Resolution = PathResolution,
                });
            }
            else
            {
                s.Segments.Add(new SplineSegment
                {
                    FirstControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = startingPos,
                        Control = startingVel,
                    },
                    SecondControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = endPos,
                    },
                    Resolution = PathResolution,
                });
            }

            s.Recalculate();
            return isValid;
        }

        private static IEnumerable<Node> ReconstructPath(Node node)
        {
            yield return node;
            if(node.Parent == null)
            {
                yield break;
            }
            foreach(var child in ReconstructPath(node.Parent))
            {
                yield return child;
            }
        }
    }
}
