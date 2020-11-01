using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VoxelExtensions
{
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
}
