using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

[RequireComponent(typeof(VoxelRenderer))]
public class DestroyableVoxel : MonoBehaviour
{
	public float ExplosionForce;
	public float ExplosionDistance;
	public int Health;

	private VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
	private VoxelMesh Mesh => Renderer.Mesh;

	private void Start()
	{
		Renderer.Mesh = Instantiate(Renderer.Mesh);
		Health = Mesh.Voxels.Values.Count(IsHealthBlock);
	}

	public static bool IsHealthBlock(Voxel v)
	{
		if(v.Coordinate.Layer > 1)
		{
			return false;
		}
		var total = (v.Material.Default.Albedo.Luminosity() + v.Material.Overrides.Sum(z => z.Data.Albedo.Luminosity()))
				/ (1 + v.Material.Overrides.Length);
		return total > 1;
	}

	public void Hit(int triindex, float force)
	{
		var voxel = Renderer.GetVoxel(triindex);
		if (!voxel.HasValue)
		{
			Debug.Log($"No voxel found.");
			return;
		}

		DestroyVoxel(voxel.Value, force, transform.position);

		Mesh.Invalidate();
		Renderer.Invalidate(false);
	}

	public void Hit(Vector3 hitPoint, Vector3 normal, float force, float radius)
	{
		StartCoroutine(HitInternal(hitPoint, normal, force, radius));
	}

	public IEnumerator HitInternal(Vector3 hitPoint, Vector3 normal, float force, float radius)
	{
		var localPos = Renderer.transform.worldToLocalMatrix.MultiplyPoint3x4(hitPoint);
		var voxels = Mesh.GetVoxels(localPos, radius).ToList();
		if (!voxels.Any())
		{
			Debug.Log($"No voxel found.");
			yield break;
		}

		foreach (var v in voxels)
		{
			Debug.Log($"Destroying a voxel: {v.Coordinate}");
			if (!Mesh.Voxels.Remove(v.Coordinate))
			{
				continue;
			}

			// Setup the single gib
			var gib = new GameObject("gib");
			gib.transform.position = transform.position;
			var r = gib.AddComponent<SingleVoxelRenderer>();
			r.Voxel = v;
			r.Invalidate();
			var bc = gib.AddComponent<BoxCollider>();
			bc.size *= .95f;
			gib.AddComponent<DestroyInTime>();
			var rb = r.gameObject.AddComponent<Rigidbody>();
			rb.gameObject.AddComponent<GravityRigidbody>();
			rb.AddExplosionForce(ExplosionForce * force, hitPoint, ExplosionDistance);
			rb.AddTorque(Vector3.one * UnityEngine.Random.value * force);
			yield return null;
		}

		Mesh.Invalidate();
		Renderer.Invalidate(false);
	}

	private void DestroyVoxel(Voxel v, float force, Vector3 hitPoint)
	{
		Debug.Log($"Destroying a voxel: {v.Coordinate}");
		if (!Mesh.Voxels.Remove(v.Coordinate))
		{
			return;
		}

		// Hit
		if(IsHealthBlock(v) && Health > 0)
		{
			Health--;
		}

		// Setup the single gib
		var gib = new GameObject("gib");
		gib.transform.position = transform.position;
		var r = gib.AddComponent<SingleVoxelRenderer>();
		r.Voxel = v;
		r.Invalidate();
		var bc = gib.AddComponent<BoxCollider>();
		bc.size *= .95f;
		gib.AddComponent<DestroyInTime>();
		var rb = r.gameObject.AddComponent<Rigidbody>();
		rb.gameObject.AddComponent<GravityRigidbody>();
		rb.AddExplosionForce(ExplosionForce * force, hitPoint, ExplosionDistance);
		rb.AddTorque(Vector3.one * UnityEngine.Random.value * force);
	}
}

