using System;
using System.Collections.Generic;

[Serializable]
public struct Voxel
{
	public VoxelMaterialData Surfaces;
	public VoxelCoordinate Coordinate;

	public Voxel(VoxelCoordinate coord, VoxelMaterialData surfaces)
	{
		Coordinate = coord;
		Surfaces = surfaces;
	}

	public IEnumerable<Voxel> Subdivide()
	{
		var newLayer = (sbyte)(Coordinate.Layer + 1);
		var centerCoord = Coordinate.ChangeLayer(newLayer) - new VoxelCoordinate(0, 0, -1, newLayer);
		for (var x = -1; x <= 1; ++x)
		{
			for (var y = -1; y <= 1; ++y)
			{
				for (var z = -2; z <= 0; ++z)
				{
					var coord = centerCoord + new VoxelCoordinate(x, y, z, newLayer);
					yield return new Voxel(coord, Surfaces.Copy());
				}
			}
		}
	}
}
