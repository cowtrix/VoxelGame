using UnityEngine;

public class Pistol : Weapon
{
	public float ExplosionForce = 1;
	protected override void OnHit(RaycastHit hit)
	{
		var destroyable = hit.collider?.GetComponent<DestroyableVoxel>();
		if (!destroyable)
		{
			return;
		}
		if(destroyable.WholeObject)
		{
			destroyable.Health--;
			return;
		}
		var voxel = destroyable.Renderer.GetVoxel(hit.triangleIndex);
		if (!voxel.HasValue)
		{
			return;
		}
		if (destroyable.DestroyVoxel(voxel.Value, ExplosionForce, destroyable.transform.position))
		{
			destroyable.Gib(ExplosionForce, hit.normal, new[] { voxel.Value });
		}
		destroyable.Mesh.Invalidate();
		destroyable.Renderer.Invalidate(false);
	}
}
