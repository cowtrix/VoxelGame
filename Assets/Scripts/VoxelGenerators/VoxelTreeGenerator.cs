using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTreeGenerator : MonoBehaviour
{
	public float GrowSpeed = 1;
	public int Steps = 6;
	[Range(0, 1)]
	public float BranchWindiness = .1f;
	[Range(0, 1)]
	public float NewBranchProbability = .5f;

	public int Depth = 2;

	public VoxelMaterial Branch;
	public VoxelMaterial Leaf;

	VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
	VoxelMesh Mesh => Renderer.Mesh;

	[ContextMenu("Refresh")]
	private void Start()
	{
		Renderer.Mesh = Renderer.Mesh ?? ScriptableObject.CreateInstance<VoxelMesh>();
		Renderer.Mesh.Voxels.Clear();
		StartCoroutine(CreateFrac(default, Depth));
	}

	private void Update()
	{
		transform.rotation *= Quaternion.Euler(0, Time.unscaledDeltaTime * 10, 0);
	}

	IEnumerator CreateFrac(VoxelCoordinate coord, int depth)
	{
		if (depth < 0)
		{
			yield break;
		}
		var vec = coord.ToVector3();
		var mat = new VoxelMaterial
		{
			Overrides = new[]
			{
				new DirectionOverride
				{
					Direction = EVoxelDirection.XNeg,
					Data = new SurfaceData
					{
						Albedo = new Color(vec.x, vec.y, vec.z),
						TextureFade = .5f,
					},
				},
				new DirectionOverride
				{
					Direction = EVoxelDirection.XPos,
					Data = new SurfaceData
					{
						Albedo = new Color(1 - vec.x, 1 - vec.y, 1 - vec.z),
						TextureFade = 1f,
					},
				},
				new DirectionOverride
				{
					Direction = EVoxelDirection.YNeg,
					Data = new SurfaceData
					{
						Albedo = new Color( vec.y * 2f, vec.x, vec.z),
						TextureFade = 1f,
					},
				},
				new DirectionOverride
				{
					Direction = EVoxelDirection.YPos,
					Data = new SurfaceData
					{
						Albedo = new Color( 1 - vec.y, 1 - vec.x, 1 - vec.z),
						TextureFade = .5f,
					},
				},
				new DirectionOverride
				{
					Direction = EVoxelDirection.ZNeg,
					Data = new SurfaceData
					{
						Albedo = new Color(vec.z, vec.y, vec.x * 2f),
						TextureFade = .5f,
					},
				},
				new DirectionOverride
				{
					Direction = EVoxelDirection.ZPos,
					Data = new SurfaceData
					{
						Albedo = new Color(1 - vec.z, 1 - vec.y, 1 - vec.x),
						TextureFade = .5f,
					},
				},
			}
		};
		if (coord.X % 3 != 0 && coord.Z % 3 != 0 && coord.Y % 3 != 0)
		{
			var v = new Voxel
			{
				Coordinate = coord,
				Material = mat,
			};
			Mesh.Voxels.SetSafe(v);
			Renderer.SetDirty();
		}
		yield return null;
		foreach (var child in coord.Subdivide())
		{
			StartCoroutine(CreateFrac(child, depth - 1));
		}
	}

	IEnumerator GrowTree(VoxelCoordinate root, Vector3 dir, int step)
	{
		var v = new Voxel
		{
			Coordinate = root,
			Material = Branch.Copy(),
		};
		Mesh.Voxels.AddSafe(v);
		Renderer.Invalidate(false);
		yield return new WaitForSeconds(.1f);

		if (Random.value < NewBranchProbability)
		{
			var newDir = new[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back }.Random();
			VoxelCoordinate.VectorToDirection(newDir, out var newDirEnum);
			var offset = VoxelCoordinate.DirectionToCoordinate(newDirEnum, root.Layer);
			yield return GrowTree(offset, newDir, Steps);
		}

		// Grow upwards
		if (step > 0)
		{
			// Grow up and maybe sideways
			int x = Random.value > BranchWindiness / 2f ? 0 : Random.Range(-1, 1);
			int z = Random.value > BranchWindiness / 2f ? 0 : Random.Range(-1, 1);
			if (x != 0 && z != 0)
			{
				// There can only be one
				if (Random.value > 0.5f)
				{
					x = 0;
				}
				else
				{
					z = 0;
				}
			}

			int y = x != 0 || z != 0 ? 0 : 1;
			yield return GrowTree(root + new VoxelCoordinate(x, y, z, root.Layer), dir, step - 1);
		}
	}
}
