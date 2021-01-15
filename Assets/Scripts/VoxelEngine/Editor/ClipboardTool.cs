using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ClipboardTool : VoxelPainterTool
{
	public enum ePasteMode
	{
		Add,    // Won't override existing voxels
		Override,   // Will override existing voxels
	}

	[Serializable]
	public class Snippet
	{
		public VoxelMapping Data;
		public Bounds Bounds => Data.Keys.GetBounds();
		public sbyte SnapLayer;

		public Snippet(string name, IEnumerable<Voxel> data)
		{
			Data = new VoxelMapping();
			foreach (var v in data)
			{
				Data.Add(v.Coordinate, v);
			}
			SnapLayer = data.Min(d => d.Coordinate.Layer);
		}

		public void DrawGUI()
		{
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.LabelField($"{Data.Count} Voxels, {Bounds}");
			EditorGUILayout.LabelField($"Snap Layer: {SnapLayer}");
			EditorGUILayout.EndVertical();
		}
	}

	public ePasteMode PasteMode;
	public Snippet CurrentClipboard;
	public Bounds SelectionBounds;
	public Vector3 Offset;

	protected override EPaintingTool ToolID => EPaintingTool.Clipboard;

	public override bool DrawInspectorGUI(VoxelPainter voxelPainter)
	{
		PasteMode = (ePasteMode)EditorGUILayout.EnumPopup("Paste Mode", PasteMode);
		if (CurrentClipboard == null)
		{
			if (GUILayout.Button("Copy Selection To Clipboard") ||
			(Event.current.isKey && Event.current.control && Event.current.keyCode == KeyCode.C))
			{
				CurrentClipboard = new Snippet("Untitled Snippet", voxelPainter.CurrentSelection.Select(c => voxelPainter.Renderer.Mesh.Voxels[c]));
				voxelPainter.CurrentSelection.Clear();
				return true;
			}
		}
		else
		{
			CurrentClipboard.DrawGUI();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("↶ 90°"))
			{
				CurrentClipboard.Data = CurrentClipboard.Data.Values.Rotate(Quaternion.Euler(0, 90, 0))
					.Finalise();
			}
			if (GUILayout.Button("↷ 90°"))
			{
				CurrentClipboard.Data =
					CurrentClipboard.Data.Values.Rotate(Quaternion.Euler(0, -90, 0))
					.Finalise();
			}
			if (GUILayout.Button("↺ 180°"))
			{
				CurrentClipboard.Data =
					CurrentClipboard.Data.Values.Rotate(Quaternion.Euler(0, 180, 0))
					.Finalise();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("⤒ Flip Vert"))
			{

			}
			if (GUILayout.Button("⇥ Flip Horizontal°"))
			{

			}
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Paste Selection To Bounds") ||
			(Event.current.isKey && Event.current.control && Event.current.keyCode == KeyCode.V))
			{
				foreach (var v in CurrentClipboard.Data)
				{
					var coord = v.Key;
					var offset = VoxelCoordinate.FromVector3(Offset, v.Key.Layer);
					var newVox = new Voxel(coord + offset, v.Value.Material);
					if (PasteMode == ePasteMode.Add)
					{
						if(!voxelPainter.Renderer.Mesh.Voxels.AddSafe(newVox))
						{
							DebugHelper.DrawCube(newVox.Coordinate, voxelPainter.Renderer.transform.localToWorldMatrix, Color.red, 5);
						}
					}
					else if (PasteMode == ePasteMode.Override)
					{
						voxelPainter.Renderer.Mesh.Voxels.SetSafe(newVox);
					}
				}
				return true;
			}
			if (GUILayout.Button("Clear Clipboard"))
			{
				CurrentClipboard = null;
			}
		}
		return false;
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent,
		List<Voxel> selection, VoxelCoordinate brushCoord, EVoxelDirection hitDir)
	{
		Handles.matrix = renderer.transform.localToWorldMatrix;
		if (CurrentClipboard == null)
		{
			if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
			{
				if (!currentEvent.shift)
				{
					voxelPainter.CurrentSelection.Clear();
				}
				if (selection == null)
				{
					return false;
				}
				var coords = selection.Select(v => v.Coordinate);
				foreach (var c in coords)
				{
					voxelPainter.CurrentSelection.Add(c);
				}
				SelectionBounds = voxelPainter.CurrentSelection.First().ToBounds();
				foreach (var p in voxelPainter.CurrentSelection.Skip(1))
				{
					SelectionBounds.Encapsulate(p.ToBounds());
				}
				if (currentEvent.control)
				{
					foreach (var v in renderer.Mesh.Voxels)
					{
						if (SelectionBounds.Contains(v.Key.ToVector3()))
						{
							voxelPainter.CurrentSelection.Add(v.Key);
						}
					}
				}
			}
			HandleExtensions.DrawWireCube(SelectionBounds.center, SelectionBounds.extents, Quaternion.identity, Color.magenta);
		}
		else
		{
			var bcenter = CurrentClipboard.Bounds.center + Offset;
			Offset = Handles.PositionHandle(bcenter, Quaternion.identity) - CurrentClipboard.Bounds.center;
			Offset = Offset.RoundToIncrement(VoxelCoordinate.LayerToScale(CurrentClipboard.SnapLayer));
			HandleExtensions.DrawWireCube(bcenter, CurrentClipboard.Bounds.extents, Quaternion.identity, Color.magenta);

			foreach(var v in CurrentClipboard.Data.Keys)
			{
				HandleExtensions.DrawWireCube(v.ToVector3() + Offset, v.ToBounds().extents, Quaternion.identity, Color.white, 0);
			}
		}

		return false;
	}
}