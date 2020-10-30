using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RemoveTool : VoxelPainterTool
{
	protected override EPaintingTool ToolID => EPaintingTool.Remove;

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord)
	{
		if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			renderer.Mesh.Voxels.Remove(brushCoord);
			return true;
		}
		return false;
	}
}