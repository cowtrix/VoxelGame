using MadMaps.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AddTool : VoxelPainterTool
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

	protected override EPaintingTool ToolID => EPaintingTool.Add;

	protected override bool GetVoxelDataFromPoint(VoxelPainter painter, VoxelRenderer renderer, Vector3 hitPoint, 
		Vector3 hitNorm, int triIndex, sbyte layer,
		out List<Voxel> selection, out VoxelCoordinate brushCoord, out EVoxelDirection hitDir)
	{
		if (Event.current.alt)
		{
			return base.GetVoxelDataFromPoint(painter, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord, out hitDir);
		}

		hitPoint = renderer.transform.worldToLocalMatrix.MultiplyPoint3x4(hitPoint);
		hitNorm = renderer.transform.worldToLocalMatrix.MultiplyVector(hitNorm);
		VoxelCoordinate.CoordinateToDirection(hitNorm, out hitDir);
		var scale = VoxelCoordinate.LayerToScale(layer);
		brushCoord = VoxelCoordinate.FromVector3(hitPoint + hitNorm * scale / 2f, layer);

		selection = null;
		return true;
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, 
		Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord, EVoxelDirection hitDir)
	{
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			if(EditorApplication.timeSinceStartup < m_lastAdd + .1f)
			{
				Debug.LogWarning($"Swallowed double event");
				return false;
			}
			m_lastAdd = EditorApplication.timeSinceStartup;
			var creationList = new HashSet<VoxelCoordinate>() { brushCoord };
			if (currentEvent.control && currentEvent.shift)
			{
				var bounds = voxelPainter.CurrentSelection.GetBounds();
				bounds.Encapsulate(brushCoord.ToBounds());
				foreach (VoxelCoordinate coord in renderer.Mesh.GetVoxelCoordinates(bounds, voxelPainter.CurrentLayer))
				{
					creationList.Add(coord);
				}
			}
			if(CreateVoxel(creationList, renderer))
			{
				voxelPainter.CurrentSelection = creationList;
			}
		}
		return false;
	}

	private bool CreateVoxel(IEnumerable<VoxelCoordinate> coords, VoxelRenderer renderer)
	{
		var coordList = coords.ToList();
		foreach (var brushCoord in coordList)
		{
			var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
			var voxelWorldPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(brushCoord.ToVector3());
			var collScale = Vector3.one * layerScale * .495f;
			collScale.Scale(renderer.transform.localToWorldMatrix.GetScale());
			var coll = Physics.OverlapBox(voxelWorldPos, collScale, renderer.transform.rotation);
			if (coll.Any())
			{
				// Collided with something, don't change
				DebugHelper.DrawCube(voxelWorldPos, collScale, renderer.transform.rotation, Color.red, 5);
				Debug.LogWarning($"Brush overlapped with {coll.First()}", coll.First());
				return false;
			}
		}
		foreach (var brushCoord in coordList)
		{
			Debug.Log($"Added new voxel at {brushCoord}");
			renderer.Mesh.Voxels[brushCoord] = new Voxel(brushCoord, CurrentBrush.Copy());
		}
		renderer.Invalidate(true);
		return true;
	}
}
