using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SelectTool : VoxelPainterTool
{
	public Bounds SelectionBounds;
	protected override EPaintingTool ToolID => EPaintingTool.Select;

	protected override bool GetVoxelDataFromPoint(VoxelPainter voxelPainterTool, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer, out List<Voxel> selection, out VoxelCoordinate brushCoord)
	{
		base.GetVoxelDataFromPoint(voxelPainterTool, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord);
		return true;
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord)
	{
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			if(!currentEvent.shift)
			{
				voxelPainter.CurrentSelection.Clear();
			}
			
			var coords = selection.Select(v => v.Coordinate);
			foreach(var c in coords)
			{
				voxelPainter.CurrentSelection.Add(c);
			}
			SelectionBounds = voxelPainter.CurrentSelection.First().ToBounds();
			foreach(var p in voxelPainter.CurrentSelection.Skip(1))
			{
				SelectionBounds.Encapsulate(p.ToBounds());
			}

			if (currentEvent.control)
			{
				foreach(var v in renderer.Mesh.Voxels)
				{
					if(SelectionBounds.Contains(v.Key.ToVector3()))
					{
						voxelPainter.CurrentSelection.Add(v.Key);
					}
				}
			}

			Debug.Log($"Added {string.Join(", ", coords)}");
		}

		Handles.matrix = renderer.transform.localToWorldMatrix;
		HandleExtensions.DrawWireCube(SelectionBounds.center, SelectionBounds.extents, Quaternion.identity, Color.magenta);

		if (voxelPainter.CurrentSelection.Any() && currentEvent.isKey && currentEvent.keyCode == KeyCode.Delete)
		{
			foreach (var c in voxelPainter.CurrentSelection)
			{
				renderer.Mesh.Voxels.Remove(c);
			}
			voxelPainter.CurrentSelection.Clear();
			SelectionBounds = default;
			return true;
		}

		return false;
	}
}
