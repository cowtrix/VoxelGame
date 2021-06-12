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
			if(!Physics.CheckBox(worldPos, targetVox.Key.GetScale() * Vector3.one, Quaternion.identity, LayerMask))
			{
				return false;
			}
		}
		return true;
	}
}
