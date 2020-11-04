using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PaintTool : VoxelPainterTool
{
	private double m_lastAdd;
	private VoxelMesh m_previewMesh;

	public override void OnEnable()
	{
		m_previewMesh = ScriptableObject.CreateInstance<VoxelMesh>();
		base.OnEnable();
	}

	public override void OnDisable()
	{
		GameObject.DestroyImmediate(m_previewMesh);
		m_previewMesh = null;
	}

	protected override bool GetVoxelDataFromPoint(VoxelPainter voxelPainterTool, VoxelRenderer renderer, 
		Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer, out List<Voxel> selection, 
		out VoxelCoordinate brushCoord, out EVoxelDirection hitDir)
	{
		var result = base.GetVoxelDataFromPoint(voxelPainterTool, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord, out hitDir);
		if (result)
		{
			Handles.matrix = renderer.transform.localToWorldMatrix;
			foreach (var s in selection)
			{
				var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
				var dirs = new HashSet<EVoxelDirection>() { hitDir };
				if(Event.current.shift)
				{
					foreach(var d in VoxelMesh.Directions)
					{
						dirs.Add(d);
					}
				}
				foreach(var d in dirs)
				{
					var rot = VoxelCoordinate.DirectionToQuaternion(d);
					var pos = s.Coordinate.ToVector3() + rot * (layerScale * .5f * Vector3.up);
					HandleExtensions.DrawWireCube(pos, new Vector3(layerScale / 2f, layerScale * .05f, layerScale / 2f), rot, Color.blue);
				}
				
			}
		}
		return result;
	}

	protected override EPaintingTool ToolID => EPaintingTool.Add;

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer,
		Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord, EVoxelDirection hitDir)
	{
		Handles.BeginGUI();
		if (currentEvent.alt)
		{
			GUI.Label(new Rect(5, 5, 180, 64),
				"PICKING\nRelease ALT to stop"
				, "Window");
		}
		else
		{
			GUI.Label(new Rect(5, 5, 180, 64),
			"ALT to change to picker", "Window");
		}
		Handles.EndGUI();

		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			if (EditorApplication.timeSinceStartup < m_lastAdd + .1f)
			{
				Debug.LogWarning($"Swallowed double event");
				return false;
			}
			m_lastAdd = EditorApplication.timeSinceStartup;
			if (currentEvent.alt)
			{
				var vox = selection.First();
				CurrentBrush = vox.Material.Copy();
				return false;
			}
			var creationList = new HashSet<VoxelCoordinate>() { brushCoord };
			/*if (currentEvent.control && currentEvent.shift)
			{
				var bounds = voxelPainter.CurrentSelection.GetBounds();
				bounds.Encapsulate(brushCoord.ToBounds());
				foreach (VoxelCoordinate coord in renderer.Mesh.GetVoxelCoordinates(bounds, voxelPainter.CurrentLayer))
				{
					creationList.Add(coord);
				}
			}*/
			if (SetVoxelSurface(creationList, renderer, hitDir, currentEvent))
			{
				voxelPainter.CurrentSelection = creationList;
			}
		}
		return false;
	}

	private bool SetVoxelSurface(IEnumerable<VoxelCoordinate> coords, VoxelRenderer renderer, EVoxelDirection dir, Event currentEvent)
	{
		var coordList = coords.ToList();
		foreach (var brushCoord in coordList)
		{
			if(!renderer.Mesh.Voxels.TryGetValue(brushCoord, out var vox))
			{
				continue;
			}
			if(currentEvent.shift)
			{
				vox.Material = CurrentBrush.Copy();
			}
			else
			{
				Debug.Log($"Set voxel at {brushCoord} ({dir})");
				var surface = CurrentBrush.GetSurface(dir);
				vox.Material.Overrides = vox.Material.Overrides.Where(o => o.Direction != dir).Append(new DirectionOverride
				{
					Direction = dir,
					Data = surface,
				}).ToArray();
			}
			renderer.Mesh.Voxels[brushCoord] = vox;
		}
		renderer.Invalidate(true);
		return true;
	}
}