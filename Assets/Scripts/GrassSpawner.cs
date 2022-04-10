using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

public class GrassSpawner : ExtendedMonoBehaviour
{
	public Texture2D VegetationTextures;
	public float TextureCellSize = 32;
	public MeshRenderer Renderer;
	public int GrassPatchCount = 100;
	public MeshFilter OutRenderer;

	List<Mesh> GetDoubleSidedPlaneMeshes()
	{
		var meshes = new List<Mesh>();
		var step = VegetationTextures.width / TextureCellSize;
		for (var x = 0f; x < 1; x += step)
		{
			for (var y = 0f; y < 1; y += step)
			{
				var m = new Mesh();
				m.SetVertices(new[]
				{
					new Vector3(-.5f, 0, 0),
					new Vector3(-.5f, 1, 0),
					new Vector3(.5f, 0, 0),
					new Vector3(.5f, 1, 0),
				});
				m.SetTriangles(new[]
				{
					0, 1, 2,
					1, 3, 2,
					2, 1, 0,
					2, 3, 1,
				}, 0);
				m.SetUVs(0, new[]
				{
					new Vector2(x, y),
					new Vector2(x, y + step),
					new Vector2(x + step, y),
					new Vector2(x + step, y + step),
				});
			}
		}
		return meshes;
	}

	[ContextMenu("Regenerate")]
	public void Regenerate()
	{
		var planeMeshes = GetDoubleSidedPlaneMeshes();
		var combineInstances = new List<CombineInstance>();
		for (var i = 0; i < GrassPatchCount; ++i)
		{
			var newInstance = new CombineInstance
			{
				mesh = planeMeshes.Random(),
				transform = Matrix4x4.TRS(new Vector3(Random.value, 0, Random.value), Quaternion.Euler(0, Random.Range(0, 360), 0), Vector3.one),
			};
			combineInstances.Add(newInstance);
		}
		if (!OutRenderer.sharedMesh)
		{
			OutRenderer.sharedMesh = new Mesh();
		}
		OutRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
	}
}
