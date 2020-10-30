using System;
using System.Collections.Generic;
using System.Linq;
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

	private static EVoxelDirection[] Directions = Enum.GetValues(typeof(EVoxelDirection)).Cast<EVoxelDirection>().ToArray();

	public Mesh GenerateMeshInstance(sbyte minLayer = sbyte.MinValue, sbyte maxLayer = sbyte.MaxValue)
	{
		Debug.Log("Generating new mesh instance");
		
		VoxelMapping.Clear();
		var m = new Mesh();
		var data = new IntermediateVoxelMeshData();
		foreach(var vox in Voxels.Where(v => v.Key.Layer >= minLayer && v.Key.Layer <= maxLayer))
		{ 
			switch (vox.Value.Material.NormalMode)
			{
				case ENormalMode.Hard:
					Cube(vox.Value, data);
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

	void Cube(Voxel vox, IntermediateVoxelMeshData data)
	{
		var origin = vox.Coordinate.ToVector3();
		var size = vox.Coordinate.GetScale() * Vector3.one;
		var dirs = Directions.ToList();
		for (int i = dirs.Count - 1; i >= 0; i--)
		{
			EVoxelDirection dir = dirs[i];
			var neighborCoord = vox.Coordinate + VoxelCoordinate.DirectionToCoordinate(dir, vox.Coordinate.Layer);
			if (Voxels.ContainsKey(neighborCoord))
			{
				dirs.RemoveAt(i);
			}
		}

		var startTri = data.Triangles.Count / 3;
		for (int i = dirs.Count - 1; i >= 0; i--)
		{
			EVoxelDirection dir = dirs[i];

			var surface = vox.Material.GetSurface(dir);
			// Get the basic mesh stuff
			VoxelMeshUtility.GetPlane(origin, size, dir, surface.UVMode, data);

			// Do the colors
			var colWithMetallic = new Color(surface.Albedo.r,
				surface.Albedo.g,
				surface.Albedo.b,
				surface.Metallic);
			data.Color1.AddRange(Enumerable.Repeat(colWithMetallic, 4));

			// UV2 extra data
			var uv2 = new Vector4(surface.Smoothness, surface.Texture.Index, vox.Coordinate.GetScale(), 0);
			data.UV2.AddRange(Enumerable.Repeat(uv2, 4));

			var endTri = data.Triangles.Count / 3;
			if(endTri > startTri)
			{
				for (var j = startTri; j < endTri; ++j)
				{
					VoxelMapping[j] = new VoxelCoordinateTriangleMapping { Coordinate = vox.Coordinate, Direction = dir };
				}
			}
		}
		
	}
}
