using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shotgun : Weapon
{
	public float Radius = 1;
	public float ExplosionForce = 1;

	protected override void OnHit(RaycastHit hit)
	{
		var destroyable = hit.collider?.GetComponent<DestroyableVoxel>();
		if (!destroyable)
		{
			return;
		}
		if (destroyable.WholeObject)
		{
			destroyable.Health--;
			return;
		}
		var localPos = destroyable.Renderer.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
		IEnumerable<Voxel> voxels = destroyable.Mesh.GetVoxels(localPos, Radius).ToList();
		if (!voxels.Any())
		{
			return;
		}
		var gibs = voxels
			.Where(vox => destroyable.DestroyVoxel(vox, ExplosionForce, hit.point))
			.ToArray()
			.Chunk(destroyable.MaxChunkSize);
		foreach (var subgib in gibs)
		{
			destroyable.Gib(ExplosionForce, hit.normal, subgib);
		}

		destroyable.Mesh.Invalidate();
		destroyable.Renderer.Invalidate(false);
	}
}