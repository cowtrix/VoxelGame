using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SubdivideTool : VoxelPainterTool
{
	protected override EPaintingTool ToolID => EPaintingTool.Subdivide;

	public override bool DrawInspectorGUI(VoxelPainter voxelPainter)
	{
		return false;
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, 
		Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord, EVoxelDirection hitDir)
	{
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			var vox = renderer.Mesh.Voxels[brushCoord];
			renderer.Mesh.Voxels.Remove(brushCoord);
			foreach (var sub in selection.SelectMany(s => s.Coordinate.Subdivide()))
			{
				renderer.Mesh.Voxels[sub] = new Voxel(sub, vox.Material.Copy());
			}
			return true;
		}
		return false;
	}

	protected override bool GetVoxelDataFromPoint(VoxelPainter painter, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer, 
		out List<Voxel> selection, out VoxelCoordinate brushCoord, out EVoxelDirection hitDir)
	{
		if(base.GetVoxelDataFromPoint(painter, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord, out hitDir))
		{
			foreach (var sub in selection.SelectMany(v => v.Coordinate.Subdivide()))
			{
				var subLayerScale = VoxelCoordinate.LayerToScale(sub.Layer);
				var subPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(sub.ToVector3());
				var subScale = renderer.transform.localToWorldMatrix.MultiplyVector(subLayerScale * Vector3.one * .4f);
				HandleExtensions.DrawWireCube(subPos, subScale, renderer.transform.rotation, Color.magenta);
			}
			return true;
		}
		return false;
	}
}
