using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VoxelRenderer))]
public class DestroyableVoxel : MonoBehaviour
{
	public float ExplosionForce;
	public float ExplosionDistance;

    private VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
    private VoxelMesh Mesh => Renderer.Mesh;

	private void Start()
	{
		Renderer.Mesh = Instantiate(Renderer.Mesh);
	}

	public void Hit(Vector3 origin, Vector3 hit, Vector3 normal, int triangleIndex)
	{
		VoxelCoordinate.VectorToDirection(normal, out var hitDir);
		var voxelN = Renderer.GetVoxel(triangleIndex);
		if(voxelN == null)
		{
			Debug.Log($"No voxel found.");
			return;
		}

		Debug.Log($"Destroying a voxel: {voxelN.Value.Coordinate}");
		Mesh.Voxels.Remove(voxelN.Value.Coordinate);
		Mesh.Invalidate();
		Renderer.Invalidate(false);

		var gib = new GameObject("gib");
		gib.transform.position = origin;
		var r = gib.AddComponent<SingleVoxelRenderer>();
		r.Voxel = voxelN.Value;
		r.Invalidate();
		var bc = gib.AddComponent<BoxCollider>();
		bc.size *= .95f;
		gib.AddComponent<DestroyInTime>();
		var rb = r.gameObject.AddComponent<Rigidbody>();
		rb.gameObject.AddComponent<GravityRigidbody>();
		rb.AddExplosionForce(ExplosionForce, origin, ExplosionDistance);
		rb.AddTorque(Vector3.one * UnityEngine.Random.value);
	}
}

