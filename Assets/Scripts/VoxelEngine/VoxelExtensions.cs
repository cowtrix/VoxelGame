using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VoxelExtensions
{
	public static VoxelMapping Finalise(this IEnumerable<Voxel> voxels)
	{
		var result = new VoxelMapping();
		foreach(var v in voxels)
		{
			result[v.Coordinate] = v;
		}
		return result;
	}

	public static IEnumerable<Voxel> Rotate(this IEnumerable<Voxel> voxels, Quaternion angle)
	{
		return voxels.Transform(v =>
		{
			var newPos = angle * v.ToVector3();			
			return VoxelCoordinate.FromVector3(newPos, v.Layer);
		});
	}

	public static IEnumerable<Voxel> Transform(this IEnumerable<Voxel> voxels, Func<VoxelCoordinate, VoxelCoordinate> func)
	{
		return voxels.Select(v =>
		{
			var newCoord = func(v.Coordinate);
			return new Voxel(newCoord, v.Material);
		});
	}

	public static Bounds GetBounds(this IEnumerable<VoxelCoordinate> voxels)
	{
		if(voxels == null || !voxels.Any())
		{
			return default;
		}
		var b = voxels.First().ToBounds();
		foreach(var b2 in voxels.Skip(1))
		{
			b.Encapsulate(b2.ToBounds());
		}
		return b;
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
}
