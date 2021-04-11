﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common;

public static class VoxelExtensions
{
	public static VoxelMapping Finalise(this IEnumerable<Voxel> voxels)
	{
		var result = new VoxelMapping();
		foreach (var v in voxels)
		{
			result[v.Coordinate] = v;
		}
		return result;
	}

	public static IEnumerable<Voxel> Offset(this IEnumerable<Voxel> voxels, Vector3 offset)
	{
		return voxels.Transform(v => v + VoxelCoordinate.FromVector3(offset, v.Layer));
	}

	public static IEnumerable<Voxel> Rotate(this IEnumerable<Voxel> voxels, Quaternion angle, Vector3 rotationCenter)
	{
		return voxels.Transform(v =>
		{
			var newPos = angle * (v.ToVector3() - rotationCenter) + rotationCenter;
			return VoxelCoordinate.FromVector3(newPos, v.Layer);
		});
	}

	public static IEnumerable<Voxel> Transform(this IEnumerable<Voxel> voxels, Func<VoxelCoordinate, VoxelCoordinate> func)
	{
		return voxels.Select(v =>
		{
			var newCoord = func(v.Coordinate);
			return new Voxel(newCoord, v.Material.Copy());
		});
	}

	public static IEnumerable<Voxel> FlipSurface(this IEnumerable<Voxel> voxels, EVoxelDirection dir)
	{
		var dAxis = dir.ToString()[0];
		var dirVals = Enum.GetValues(typeof(EVoxelDirection)).Cast<EVoxelDirection>();
		return voxels.Select(v =>
		{
			v.Material = v.Material.Copy();
			for (int i = 0; i < v.Material.Overrides.Length; i++)
			{
				var o = v.Material.Overrides[i];
				var oAxis = o.Direction.ToString()[0];
				if (oAxis != dAxis)
				{
					continue;
				}
				o.Direction = dirVals.First(d => d != o.Direction && d.ToString()[0] == oAxis);
				v.Material.Overrides[i] = o;
			}
			return v;
		});
	}

	public static Bounds GetBounds(this IEnumerable<VoxelCoordinate> voxels)
	{
		if (voxels == null || !voxels.Any())
		{
			return default;
		}
		var b = voxels.First().ToBounds();
		foreach (var b2 in voxels.Skip(1))
		{
			b.Encapsulate(b2.ToBounds());
		}
		return b;
	}

	public static IEnumerable<Voxel> Optimise(this IEnumerable<Voxel> voxels)
	{
		var allVoxels = voxels.ToList();
		for (int i = allVoxels.Count - 1; i >= 0; i--)
		{
			var voxel = allVoxels[i];
			var parentCoord = voxel.Coordinate.ChangeLayer((sbyte)(voxel.Coordinate.Layer - 1));
		}
		return voxels;
	}

	public static bool CollideCheck(this IEnumerable<VoxelCoordinate> voxels, VoxelCoordinate coord, out VoxelCoordinate collision)
	{
		if (voxels.Contains(coord))
		{
			collision = coord;
			return true;
		}
		var delta = VoxelCoordinate.LayerToScale(coord.Layer) * .01f;
		var b1 = coord.ToBounds();
		b1.Expand(-delta);
		foreach (var vox in voxels)
		{
			var b2 = vox.ToBounds();
			b2.Expand(-delta);
			if (b1.Intersects(b2))
			{
				collision = vox;
				return true;
			}
		};
		collision = default;
		return false;
	}

	public static IEnumerable<IEnumerable<Voxel>> Chunk(this IEnumerable<Voxel> inVoxels, int chunkSize)
	{
		List<List<Voxel>> result = new List<List<Voxel>>();
		foreach(var v in inVoxels)
		{
			List<Voxel> bestList = result
				.Where(l => l.Count < chunkSize && l.Select(s => s.Coordinate).IsConnected(v.Coordinate))
				.FirstOrDefault();
			if(bestList == null)
			{
				bestList = new List<Voxel>();
				result.Add(bestList);
			}
			bestList.Add(v);
		}
		return result;
	}

	public static bool IsConnected(this IEnumerable<VoxelCoordinate> inVoxels, VoxelCoordinate coord) =>
		inVoxels.Any(v => coord.IsNeighbour(v));

	public static bool IsNeighbour(this VoxelCoordinate coord1, VoxelCoordinate coord)
	{
		return ManhattenDistance(coord1, coord) <= VoxelCoordinate.LayerToScale(Math.Max(coord1.Layer, coord.Layer));
	}

	public static float ManhattenDistance(this VoxelCoordinate coord1, VoxelCoordinate coord)
	{
		return coord.ToVector3().ManhattenDistance(coord1.ToVector3());
	}
}
