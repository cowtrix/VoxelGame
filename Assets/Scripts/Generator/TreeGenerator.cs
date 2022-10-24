using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using UnityEditor;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Voxul.Utilities;

namespace Generation
{
	public class TreeGenerator : DynamicVoxelGenerator
	{
		public enum EGenerationMode
		{
			Trunk,
			Leaf,
			Root,
		}

        [Serializable]
        public class TreeParameters
		{
            [Range(0, 1)]
            public float Gravity = .75f;
            public AnimationCurve Growth = AnimationCurve.Linear(1, 1, 1, 1);
            [Range(0, 1)]
            public float BranchProbability = .1f;
            [Range(0, 1)]
            public float LeafProbability = .1f;
            public VoxelBrush BranchMaterial, LeafMaterial;
        }

		public TreeParameters Parameters;
		public LayerMask CollisionMask;
		public int MaxVoxels = 1000;
		[Range(VoxelCoordinate.MIN_LAYER, VoxelCoordinate.MAX_LAYER)]
		public sbyte Layer;

		protected override void SetVoxels(VoxelRenderer renderer)
		{
			var gravDir = EVoxelDirection.YPos;
			var branchDirs = new[] { EVoxelDirection.XNeg, EVoxelDirection.XPos, EVoxelDirection.ZNeg, EVoxelDirection.ZPos };
			var leafDirs = new[] { EVoxelDirection.YNeg,
				EVoxelDirection.YPos, EVoxelDirection.XNeg, EVoxelDirection.XPos, EVoxelDirection.ZNeg, EVoxelDirection.ZPos,
				EVoxelDirection.YPos, EVoxelDirection.XNeg, EVoxelDirection.XPos, EVoxelDirection.ZNeg, EVoxelDirection.ZPos };
			var rnd = new System.Random();
			var open = new Queue<(EGenerationMode, VoxelCoordinate)>();

			var history = new Dictionary<VoxelCoordinate, EGenerationMode>();

			open.Enqueue((EGenerationMode.Trunk, new VoxelCoordinate(0, 0, 0, Layer)));
			while (open.Any() && renderer.Mesh.Voxels.Count < MaxVoxels)
			{
				var current = open.Dequeue();
				
				switch (current.Item1)
				{
					case EGenerationMode.Trunk:
						{
							// Roll branch
							if (rnd.NextDouble() < Parameters.BranchProbability)
							{
								var branchPos = current.Item2 + VoxelCoordinate.DirectionToCoordinate(branchDirs.Random(), current.Item2.Layer);
								if(history.ContainsKey(branchPos) || Physics.CheckBox(branchPos.ToVector3(), VoxelCoordinate.LayerToScale( current.Item2.Layer) * Vector3.one * .9f, transform.rotation, CollisionMask))
								{
									DebugHelper.DrawCube(branchPos.ToVector3(), VoxelCoordinate.LayerToScale(current.Item2.Layer) * Vector3.one * .9f, Quaternion.identity, Color.red, 1);
									continue;
								}
								if (rnd.NextDouble() < Parameters.LeafProbability)
								{
									history.Add(branchPos, EGenerationMode.Leaf);
                                    open.Enqueue((EGenerationMode.Leaf, branchPos));
								}
								else
								{
									var trunkCount = 0;
									foreach(var neighbour in branchPos.GetNeighbours())
									{
										if(history.TryGetValue(neighbour, out var neighbourType) && neighbourType == EGenerationMode.Trunk)
										{
											trunkCount++;
                                        }
									}
									if(trunkCount > 3)
									{
										continue;
									}
                                    history.Add(branchPos, EGenerationMode.Trunk);
                                    open.Enqueue((EGenerationMode.Trunk, branchPos));
								}
							}
						}
						{
							// Grow branch
							var nextDir = rnd.NextDouble() > Parameters.Gravity ? gravDir : branchDirs.Random();
							var nextCoord = current.Item2 + VoxelCoordinate.DirectionToCoordinate(nextDir, current.Item2.Layer);
							if (history.ContainsKey(nextCoord) || rnd.NextDouble() > Parameters.Growth.Evaluate(renderer.Mesh.Voxels.Count / (float)MaxVoxels))
							{
								continue;
							}
                            history.Add(nextCoord, EGenerationMode.Leaf);
                            open.Enqueue((EGenerationMode.Trunk, nextCoord));
						}
						renderer.Mesh.Voxels.AddSafe(new Voxel(current.Item2, Parameters.BranchMaterial.Generate((float)rnd.NextDouble())));
						break;
					case EGenerationMode.Leaf:
						foreach (var dir in leafDirs)
						{
							if (rnd.NextDouble() > Parameters.LeafProbability)
							{
								continue;
							}
							var leafPos = current.Item2 + VoxelCoordinate.DirectionToCoordinate(dir, current.Item2.Layer);
                            if (history.ContainsKey(leafPos) || Physics.CheckBox(leafPos.ToVector3(), VoxelCoordinate.LayerToScale(current.Item2.Layer) * Vector3.one * .9f, transform.rotation, CollisionMask))
                            {
                                continue;
                            }
                            history.Add(leafPos, EGenerationMode.Leaf);
                            open.Enqueue((EGenerationMode.Leaf, leafPos));
						}
						renderer.Mesh.Voxels.AddSafe(new Voxel(current.Item2, Parameters.LeafMaterial.Generate((float)rnd.NextDouble())));
						break;
				}

			}
		}
	}
}
