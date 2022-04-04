using Interaction.Items;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Voxul.Utilities;

public class AsteroidGenerator : DynamicVoxelGenerator
{
	public Bounds Bounds;
	public AnimationCurve Falloff = AnimationCurve.Linear(0, 1, 1, 0);
	[Range(VoxelCoordinate.MIN_LAYER, VoxelCoordinate.MAX_LAYER)]
	public sbyte Layer;
	public float Frequency = 1;
	public float Magnitude = 1;
	public int Octaves = 3;

	[Range(0, 1)]
	public float Cuttoff;
	public VoxelBrush DefaultBrush;
	public List<AsteroidResource> Resources;
	[Serializable]
	public class AsteroidResource
	{
		public VoxelBrush Brush;
		public IntResourceDelta Delta;
		[Range(0, 1)]
		public float Amount;
		[HideInInspector]
		public List<VoxelCoordinate> CurrentCoordinates = new List<VoxelCoordinate>();
	}

	protected override void SetVoxels(VoxelRenderer renderer)
	{
		var resourceSum = Resources.Sum(r => r.Amount) + 1;

		// Noise fill
		var rnd = new System.Random();
		var randomOffset = new Vector3(rnd.Next(-5000, 5000), rnd.Next(-5000, 5000), rnd.Next(-5000, 5000));
		var layerScale = VoxelCoordinate.LayerToScale(Layer);
		foreach (var rsc in Resources)
		{
			rsc.CurrentCoordinates.Clear();
		}

		var allTasks = new List<Task>();
		var voxels = new ConcurrentBag<Voxel>();
		for (var x = Bounds.min.x; x < Bounds.min.x + Bounds.size.x; x += layerScale)
		{
			for (var y = Bounds.min.y; y < Bounds.min.y + Bounds.size.y; y += layerScale)
			{
				for (var z = Bounds.min.z; z < Bounds.min.z + Bounds.size.z; z += layerScale)
				{
					//allTasks.Add(Task.Factory.StartNew(() =>
					{
						var value = Mathf.Abs(Perlin.Fbm(x * Frequency + randomOffset.x, y * Frequency + randomOffset.y, z * Frequency + randomOffset.z, Octaves))
							* Magnitude;

						var falloffPositions = new Vector3(Mathf.Abs(x - Bounds.center.x) / Bounds.extents.x, Mathf.Abs(y - Bounds.center.y) / Bounds.extents.y, Mathf.Abs(z - Bounds.center.z) / Bounds.extents.z);
						var cuttoffFactor = Falloff.Evaluate(falloffPositions.x) * Falloff.Evaluate(falloffPositions.y) * Falloff.Evaluate(falloffPositions.z);
						value *= cuttoffFactor;
						if (value < Cuttoff)
						{
							continue;
						}
						var rscRoll = rnd.NextDouble() * resourceSum;
						var brush = DefaultBrush;
						AsteroidResource rsc = null;
						if (rscRoll >= 1)
						{
							var counter = 0f;
							foreach (var r in Resources)
							{
								brush = r.Brush;
								rsc = r;
								if (counter + r.Amount > rscRoll - 1)
								{
									break;
								}
								counter += r.Amount;
							}
						}
						var voxelCoordinate = VoxelCoordinate.FromVector3(x, y, z, Layer);
						if (rsc != null)
						{
							rsc.CurrentCoordinates.Add(voxelCoordinate);
						}
						voxels.Add(new Voxel(voxelCoordinate, brush.Generate(Mathf.Clamp01(value))));
					}
					//));
				}
			}
		}
		//Task.WaitAll(allTasks.ToArray());
		foreach(var vox in voxels)
		{
			renderer.Mesh.Voxels[vox.Coordinate] = vox;
		}

		// Flood fill remove floaters
		foreach (var voxCoord in renderer.Mesh.Voxels.Keys.ToList())
		{
			if (!renderer.Mesh.Voxels.ContainsKey(voxCoord))
			{
				continue;
			}
			if (!renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(0, 1, 0, voxCoord.Layer))
				&& !renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(0, -1, 0, voxCoord.Layer))
				&& !renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(1, 0, 0, voxCoord.Layer))
				&& !renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(-1, 0, 0, voxCoord.Layer))
				&& !renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(0, 0, 1, voxCoord.Layer))
				&& !renderer.Mesh.Voxels.ContainsKey(voxCoord + new VoxelCoordinate(0, 0, -1, voxCoord.Layer)))
			{
				renderer.Mesh.Voxels.Remove(voxCoord);
			}

			int emptyCount = 0;
			foreach (var neighbour in voxCoord.GetNeighbours())
			{
				if (!renderer.Mesh.Voxels.ContainsKey(neighbour))
				{
					emptyCount++;
					continue;
				}
				if (rnd.NextDouble() < .5)
				{
					renderer.Mesh.Voxels[voxCoord] = new Voxel { Coordinate = voxCoord, Material = renderer.Mesh.Voxels[neighbour].Material.Copy() };
				}
			}
			if (emptyCount > 5)
			{
				renderer.Mesh.Voxels.Remove(voxCoord);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}