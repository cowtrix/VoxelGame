using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SubdivideTool : VoxelPainterTool
{
	protected override EPaintingTool ToolID => EPaintingTool.Subdivide;

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord)
	{
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			renderer.Mesh.Voxels.Remove(brushCoord);
			foreach (var sub in selection.SelectMany(s => s.Subdivide()))
			{
				renderer.Mesh.Voxels[sub.Coordinate] = sub;
			}
			return true;
		}
		return false;
	}

	protected override bool GetVoxelDataFromPoint(VoxelPainter painter, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer, 
		out List<Voxel> selection, out VoxelCoordinate brushCoord)
	{
		if(base.GetVoxelDataFromPoint(painter, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord))
		{
			foreach (var sub in selection.SelectMany(v => v.Subdivide()))
			{
				var subLayerScale = VoxelCoordinate.LayerToScale(sub.Coordinate.Layer);
				var subPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(sub.Coordinate.ToVector3());
				var subScale = renderer.transform.localToWorldMatrix.MultiplyVector(subLayerScale * Vector3.one * .4f);
				HandleExtensions.DrawWireCube(subPos, subScale, renderer.transform.rotation, Color.magenta);
			}
			return true;
		}
		return false;
	}
}
