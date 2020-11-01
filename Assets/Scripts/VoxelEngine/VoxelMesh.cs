using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class IntermediateVoxelMeshData
{
	public List<Vector3> Vertices = new List<Vector3>();
	public List<int> Triangles = new List<int>();
	public List<Color> Color1 = new List<Color>();
	public List<Vector2> UV1 = new List<Vector2>();
	public List<Vector4> UV2 = new List<Vector4>();
}

[Serializable]
public struct VoxelCoordinateTriangleMapping
{
	public VoxelCoordinate Coordinate;
	public EVoxelDirection Direction;
}

[Serializable]
public class TriangleVoxelMapping : SerializableDictionary<int, VoxelCoordinateTriangleMapping> { }
[Serializable]
public class VoxelMapping : SerializableDictionary<VoxelCoordinate, Voxel> { }

[CreateAssetMenu]
public class VoxelMesh : ScriptableObject
{
	[HideInInspector]
	public string Hash;
	[HideInInspector]
	public TriangleVoxelMapping VoxelMapping = new TriangleVoxelMapping();
	public VoxelMapping Voxels = new VoxelMapping();

	public static EVoxelDirection[] Directions = Enum.GetValues(typeof(EVoxelDirection)).Cast<EVoxelDirection>().ToArray();

	public Mesh GenerateMeshInstance(sbyte minLayer = sbyte.MinValue, sbyte maxLayer = sbyte.MaxValue)
	{		
		VoxelMapping.Clear();
		var m = new Mesh();
		var data = new IntermediateVoxelMeshData();
		foreach(var vox in Voxels.Where(v => v.Key.Layer >= minLayer && v.Key.Layer <= maxLayer))
		{ 
			switch (vox.Value.Material.RenderMode)
			{
				case ERenderMode.Block:
					Cube(vox.Value, data);
					break;
				case ERenderMode.XPlane:
					Plane(vox.Value, data, new[] { EVoxelDirection.XPos, EVoxelDirection.XNeg, });
					break;
				case ERenderMode.YPlane:
					Plane(vox.Value, data, new[] { EVoxelDirection.YPos, EVoxelDirection.YNeg, });
					break;
				case ERenderMode.ZPlane:
					Plane(vox.Value, data, new[] { EVoxelDirection.ZPos, EVoxelDirection.ZNeg, });
					break;
				case ERenderMode.XYCross:
					Plane(vox.Value, data, new[] { EVoxelDirection.XPos, EVoxelDirection.XNeg, EVoxelDirection.YPos, EVoxelDirection.YNeg, });
					break;
				case ERenderMode.XZCross:
					Plane(vox.Value, data, new[] { EVoxelDirection.XPos, EVoxelDirection.XNeg, EVoxelDirection.ZPos, EVoxelDirection.ZNeg, });
					break;
				case ERenderMode.ZYCross:
					Plane(vox.Value, data, new[] { EVoxelDirection.ZPos, EVoxelDirection.ZNeg, EVoxelDirection.YPos, EVoxelDirection.YNeg,});
					break;
				case ERenderMode.FullCross:
					Plane(vox.Value, data, Directions.ToArray());
					break;
			}
		}
		m.SetVertices(data.Vertices);
		m.SetColors(data.Color1);
		m.SetTriangles(data.Triangles, 0);
		m.SetUVs(0, data.UV1);
		m.SetUVs(1, data.UV2);
		m.RecalculateNormals();

		return m;
	}

	public IEnumerable<VoxelCoordinate> GetVoxelCoordinates(Bounds bounds, sbyte currentLayer)
	{
		var minCoord = VoxelCoordinate.FromVector3(bounds.min, currentLayer);
		var maxCoord = VoxelCoordinate.FromVector3(bounds.max, currentLayer);

		for(var x = minCoord.X; x < maxCoord.X; ++x)
		{
			for (var y = minCoord.Y; y < maxCoord.Y; ++y)
			{
				for (var z = minCoord.Z; z < maxCoord.Z; ++z)
				{
					yield return new VoxelCoordinate(x, y, z, currentLayer);
				}
			}
		}
	}

	public IEnumerable<Voxel> GetVoxels(Bounds bounds)
	{
		return Voxels
			.Where(v => bounds.Contains(v.Key.ToVector3()))
			.Select(v => v.Value);
	}

	private void Plane(Voxel vox, IntermediateVoxelMeshData data, IEnumerable<EVoxelDirection> dirs)
	{
		var origin = vox.Coordinate.ToVector3();
		var size = vox.Coordinate.GetScale() * Vector3.one;
		DoPlanes(origin, 0, size.xz(), dirs, vox, data);
	}

	private void Cube(Voxel vox, IntermediateVoxelMeshData data)
	{
		var origin = vox.Coordinate.ToVector3();
		var size = vox.Coordinate.GetScale() * Vector3.one;
		var dirs = Directions.ToList();
		for (int i = dirs.Count - 1; i >= 0; i--)
		{
			EVoxelDirection dir = dirs[i];
			var neighborCoord = vox.Coordinate + VoxelCoordinate.DirectionToCoordinate(dir, vox.Coordinate.Layer);
			if (Voxels.TryGetValue(neighborCoord, out var n) && n.Material.RenderMode == ERenderMode.Block)
			{
				dirs.RemoveAt(i);
			}
		}
		DoPlanes(origin, size.y, size.xz(), dirs, vox, data);
	}

	private void DoPlanes(Vector3 origin, float offset, Vector2 size,
		IEnumerable<EVoxelDirection> dirs, Voxel vox, IntermediateVoxelMeshData data)
	{
		var startTri = data.Triangles.Count / 3;
		foreach(var dir in dirs)
		{ 
			var surface = vox.Material.GetSurface(dir);
			// Get the basic mesh stuff
			VoxelMeshUtility.GetPlane(origin, offset, size, dir, surface.UVMode, data);

			// Do the colors
			var colWithMetallic = new Color(surface.Albedo.r,
				surface.Albedo.g,
				surface.Albedo.b,
				surface.Metallic);
			data.Color1.AddRange(Enumerable.Repeat(colWithMetallic, 4));

			// UV2 extra data
			var uv2 = new Vector4(surface.Smoothness, surface.Texture.Index, vox.Coordinate.GetScale(), 1 - surface.TextureFade);
			data.UV2.AddRange(Enumerable.Repeat(uv2, 4));

			var endTri = data.Triangles.Count / 3;
			if (endTri > startTri)
			{
				for (var j = startTri; j < endTri; ++j)
				{
					VoxelMapping[j] = new VoxelCoordinateTriangleMapping { Coordinate = vox.Coordinate, Direction = dir };
				}
			}
		}
	}
}
