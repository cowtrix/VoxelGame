﻿using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SingleVoxelRenderer : MonoBehaviour
{
	public Voxel Voxel;
	private MeshRenderer Renderer => GetComponent<MeshRenderer>();
	private MeshFilter MeshFilter => GetComponent<MeshFilter>();

	[ContextMenu("Invalidate")]
	public void Invalidate()
	{
		Renderer.sharedMaterial = VoxelManager.Instance.DefaultMaterial;
		var data = new IntermediateVoxelMeshData(null);
		VoxelMesh.Cube(Voxel, data);
		MeshFilter.sharedMesh = data.SetMesh(MeshFilter.sharedMesh);
	}
}
