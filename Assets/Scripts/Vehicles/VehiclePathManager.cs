using Splines;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

namespace Vehicles.AI
{
    public class VehiclePathManager : Singleton<VehiclePathManager>
    {
        public static VehiclePathNode GetClosestNode(Vector3 position, Vector3 velocity, out int laneIndex)
        {
            foreach(var node in VehiclePathNode.Instances
                .OrderBy(n => Vector3.Distance(position, n.transform.position)))
            {
                var bestLane = node.Lanes.OrderBy(l => 
                    (Vector3.Angle(velocity.normalized, node.transform.localToWorldMatrix.MultiplyVector(l.Normal.normalized))))
                    .First();
                laneIndex = node.Lanes.IndexOf(bestLane);
                return node;
                /*for (int i = 0; i < node.Lanes.Count; i++)
                {
                    VehiclePathLane lane = node.Lanes[i];
                    if ( > .5f)
                    {
                        laneIndex = i;
                        return node;
                    }
                }*/
            }
            laneIndex = -1;
            return null;
        }

        public float PathResolution = .2f;

        private class Node
        {
            public VehiclePathNode Component { get; private set; }
            public int LaneIndex { get; private set; }
            public Vector3 Position { get; private set; }
            public Node Parent { get; set; }
            public List<Node> Connections { get; private set; } = new List<Node>();
            public float AccumulatedCost { get; set; }
            public float DistanceToTarget { get; set; }
            public float TotalScore => AccumulatedCost + DistanceToTarget;
            public Node(VehiclePathNode node, int laneIndex)
            {
                Component = node;
                LaneIndex = laneIndex;
                Position = Component.transform.position + Component.transform.localToWorldMatrix.MultiplyVector(Component.Lanes[laneIndex].Offset);
            }
        }

        public Spline GetPath(Vector3 startingPos, Vector3 startingVel, Vector3 endPos, float laneJoinDistance = 50f)
        {
            var s = new Spline();
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
                    n.DistanceToTarget = Vector3.Distance(n.Position, endPos);
                    open.Add(n);
                }

                while (open.Any())
                {
                    var next = open.OrderBy(n => n.TotalScore).First();
                    open.Remove(next);

                    if (open.Any(n => n == next && n.TotalScore < next.TotalScore))
                    {
                        continue;
                    }

                    closed.Add(next);

                    if (next.Component == closestNodeToEnd)
                    {
                        break;
                    }

                    var lane = next.Component.Lanes[laneIndex];
                    if (!lane.Node.gameObject.activeInHierarchy || closed.Any(n => n.Component == lane.Node))
                    {
                        continue;
                    }
                    var newNode = new Node(lane.Node, laneIndex) { Parent = next };
                    newNode.DistanceToTarget = Vector3.Distance(newNode.Position, endPos);
                    if (newNode.Parent != null)
                    {
                        newNode.AccumulatedCost = newNode.Parent.AccumulatedCost
                            + Vector3.Distance(next.Component.transform.position, newNode.Component.transform.position);
                    }
                    next.Connections.Add(newNode);
                    open.Add(newNode);
                }

                var path = ReconstructPath(closed.Single(c => c.Parent == null))
                    .ToList();

                VehiclePathNode lastNode = null;
                for (int i = 0; i < path.Count; i++)
                {
                    var n = path[i];
                    if (i > 0)
                    {
                        s.Segments.Add(new SplineSegment
                        {
                            FirstControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = lastNode.GetWorldLanePosition(laneIndex),
                                Control = -lastNode.GetWorldNormal(laneIndex),
                            },
                            SecondControlPoint = new SplineSegment.ControlPoint
                            {
                                Position = n.GetWorldLanePosition(laneIndex),
                                Control = n.GetWorldNormal(laneIndex),
                            },
                            Resolution = PathResolution,
                        });
                    }
                    lastNode = n;
                }

                s.Segments.Add(new SplineSegment
                {
                    FirstControlPoint = new SplineSegment.ControlPoint
                    {
                        Position = lastNode.GetWorldLanePosition(laneIndex),
                        Control = -lastNode.GetWorldNormal(laneIndex),
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
            return s;
        }

        private static IEnumerable<VehiclePathNode> ReconstructPath(Node node)
        {
            yield return node.Component;
            if (node.Connections == null || !node.Connections.Any())
            {
                yield break;
            }
            var bestConnection = node.Connections.OrderBy(c => c.TotalScore).First();
            foreach (var children in ReconstructPath(bestConnection))
            {
                yield return children;
            }
        }
    }
}
