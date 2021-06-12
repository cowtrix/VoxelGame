using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

public class LevelWin : TrackedObject<LevelWin>
{
	public Checkpoint NextCheckpoint;
	public LayerMask LayerMask;
	protected VoxelRenderer Renderer => GetComponent<VoxelRenderer>();

    public bool CheckWin(Player p)
	{
		var pBounds = p.Renderer.Bounds;
		var b = Renderer.Bounds;
		if (!pBounds.Intersects(b))
		{
			return false;
		}		
		foreach(var targetVox in Renderer.Mesh.Voxels)
		{
			var worldPos = transform.localToWorldMatrix.MultiplyPoint3x4(targetVox.Key.ToVector3());
			var localPos = VoxelCoordinate.FromVector3(p.transform.worldToLocalMatrix.MultiplyPoint3x4(worldPos), targetVox.Key.Layer);
			if (!p.Renderer.Mesh.Voxels.ContainsKey(localPos))
			{
				return false;
			}
		}
		return true;
	}
}
