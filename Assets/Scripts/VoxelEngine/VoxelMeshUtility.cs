using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VoxelMeshUtility
{
	public static void GetPlane(Vector3 origin, Vector3 size, EVoxelDirection dir, IntermediateVoxelMeshData data)
	{
		var cubeLength = size.x;
		var cubeWidth = size.z;
		var cubeHeight = size.z;

		Quaternion rot = VoxelCoordinate.DirectionToQuaternion(dir);
		
		// Vertices
		Vector3 v1 = origin + rot * new Vector3(-cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
		Vector3 v2 = origin + rot * new Vector3(cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
		Vector3 v3 = origin + rot * new Vector3(cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
		Vector3 v4 = origin + rot * new Vector3(-cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
		var vOffset = data.Vertices.Count;
		data.Vertices.AddRange(new[]
		{
			v1, v2, v3, v4
		});

		// Triangles
		data.Triangles.AddRange(new[]
		{
			// Cube Left Side Triangles
			3 + vOffset, 1 + vOffset, 0 + vOffset,
			3 + vOffset, 2 + vOffset, 1 + vOffset,
		});

		// UV1
		Vector2 _00_CORDINATES = new Vector2(0f, 0f);
		Vector2 _10_CORDINATES = new Vector2(1f, 0f);
		Vector2 _01_CORDINATES = new Vector2(0f, 1f);
		Vector2 _11_CORDINATES = new Vector2(1f, 1f);
		data.UV1.AddRange(new[]
		{
			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
		});
	}
}
