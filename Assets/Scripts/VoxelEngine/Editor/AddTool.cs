using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AddTool : VoxelPainterTool
{
	private static Editor m_cachedBrushEditor;
	private static bool m_cachedEditorNeedsRefresh = true;
	public VoxelMaterial CurrentBrush;

	protected override EPaintingTool ToolID => EPaintingTool.Add;

	protected override bool GetVoxelDataFromPoint(VoxelPainter painter, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer,
		out List<Voxel> selection, out VoxelCoordinate brushCoord)
	{
		var scale = VoxelCoordinate.LayerToScale(layer);
		brushCoord = VoxelCoordinate.FromVector3(hitPoint + hitNorm * scale / 2f, layer);
		selection = null;
		return true;
	}

	public override void DrawInspectorGUI(VoxelPainter voxelPainter)
	{
		var newBrush = (VoxelMaterial)EditorGUILayout.ObjectField("Current Brush", CurrentBrush, typeof(VoxelMaterial), false);
		var dirty = newBrush != CurrentBrush;
		CurrentBrush = newBrush;
		if (CurrentBrush)
		{
			if (dirty || m_cachedBrushEditor == null || m_cachedEditorNeedsRefresh)
			{
				m_cachedBrushEditor = Editor.CreateEditor(CurrentBrush);
				m_cachedEditorNeedsRefresh = false;
			}
			m_cachedBrushEditor?.DrawDefaultInspector();
		}
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord)
	{
		if (!CurrentBrush)
		{
			return false;
		}
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
			var voxelWorldPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(brushCoord.ToVector3());
			var collScale = Vector3.one * layerScale * .495f;
			var coll = Physics.OverlapBox(voxelWorldPos, collScale, renderer.transform.rotation);
			if (coll.Any())
			{
				// Collided with something, don't change
				DebugHelper.DrawCube(voxelWorldPos, collScale, renderer.transform.rotation, Color.red, 5);
				return false;
			}
			renderer.Mesh.Voxels[brushCoord] = new Voxel(brushCoord, CurrentBrush.Data.Copy());
			renderer.Invalidate();
			return true;
		}
		return false;
	}
}