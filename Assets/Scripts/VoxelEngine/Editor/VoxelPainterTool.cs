using JetBrains.Annotations;
using MadMaps.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class VoxelPainterTool
{
	protected abstract EPaintingTool ToolID { get; }

	protected virtual bool GetVoxelDataFromPoint(VoxelPainter voxelPainterTool, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer,
		out List<Voxel> selection, out VoxelCoordinate brushCoord)
	{
		var voxelN = renderer.GetVoxel(triIndex);
		if (voxelN.HasValue)
		{
			selection = new List<Voxel> { voxelN.Value };
			brushCoord = voxelN.Value.Coordinate;
			return true;
		}

		selection = null;
		brushCoord = default;
		return false;
	}

	public void DrawSceneGUI(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, sbyte painterLayer)
	{
		if (voxelPainter.CurrentSelection != null)
		{
			// Show selection handles
			foreach (var v in voxelPainter.CurrentSelection)
			{
				var pos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(v.ToVector3());
				var scale = renderer.transform.localToWorldMatrix.MultiplyVector(VoxelCoordinate.LayerToScale(v.Layer) * Vector3.one * .51f);
				HandleExtensions.DrawWireCube(pos, scale, renderer.transform.rotation, Color.white);
			}
		}

		var collider = renderer.GetComponent<MeshCollider>();
		Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hitPoint = Vector3.zero;
		var hitNorm = Vector3.up;
		var triIndex = -1;
		if (collider.Raycast(worldRay, out var hitInfo, 10000))
		{
			hitPoint = hitInfo.point;
			hitNorm = hitInfo.normal;
			triIndex = hitInfo.triangleIndex;
		}
		else
		{
			var p = new Plane(renderer.transform.up, renderer.transform.position.y); ;
			if (p.Raycast(worldRay, out var planePoint))
			{
				hitPoint = worldRay.origin + worldRay.direction * planePoint;
				hitNorm = renderer.transform.up; ;
			}
		}
		Handles.DrawWireCube(hitPoint, Vector3.one * .02f);
		Handles.DrawLine(hitPoint, hitPoint + hitNorm * .2f);

		if (!GetVoxelDataFromPoint(voxelPainter, renderer, hitPoint, hitNorm, triIndex, painterLayer,
			out var selection, out var brushCoord))
		{
			return;
		}

		var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
		var voxelWorldPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(brushCoord.ToVector3());
		var voxelScale = layerScale * Vector3.one * .51f;
		voxelScale.Scale(renderer.transform.localToWorldMatrix.GetScale());
		HandleExtensions.DrawWireCube(voxelWorldPos, voxelScale, renderer.transform.rotation, Color.cyan);
		Handles.Label(voxelWorldPos, brushCoord.ToString(), EditorStyles.textField);

		if (DrawSceneGUIInternal(voxelPainter, renderer, currentEvent, selection, brushCoord))
		{
			renderer.Mesh.Hash = System.Guid.NewGuid().ToString();
			EditorUtility.SetDirty(renderer.Mesh);
			Event.current.Use();
			Debug.LogWarning("Used event");
		}
	}

	public virtual void DrawInspectorGUI(VoxelPainter voxelPainter)
	{
	}

	protected abstract bool DrawSceneGUIInternal(VoxelPainter painter, VoxelRenderer Renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord);

	public virtual void OnEnable()
	{
	}
}