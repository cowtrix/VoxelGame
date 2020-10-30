using ICSharpCode.NRefactory.Ast;
using MadMaps.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public enum EPaintingTool
{
	Add,
	Remove,
	Subdivide,
}


[CustomEditor(typeof(VoxelRenderer))]
public class VoxelPainter : Editor
{
	public bool Enabled
	{
		get
		{
			return EditorPrefUtility.GetPref("VoxelPainter_Enabled", true);
		}
		set
		{
			EditorPrefUtility.SetPref("VoxelPainter_Enabled", value);
		}
	}
	public sbyte CurrentLayer
	{
		get
		{
			return EditorPrefUtility.GetPref("VoxelPainter_CurrentLayer", default(sbyte));
		}
		set
		{
			EditorPrefUtility.SetPref("VoxelPainter_CurrentLayer", value);
		}
	}
	public EPaintingTool CurrentTool
	{
		get
		{
			return EditorPrefUtility.GetPref("VoxelPainter_CurrentTool", EPaintingTool.Add);
		}
		set
		{
			EditorPrefUtility.SetPref("VoxelPainter_CurrentTool", value);
		}
	}

	private Editor m_cachedBrushEditor;
	private bool m_cachedEditorNeedsRefresh = true;

	public VoxelMaterial CurrentBrush;
	private VoxelRenderer Renderer => target as VoxelRenderer;

	private Camera m_cam => Camera.current;

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
		GUI.enabled = Enabled;
		CurrentLayer = (sbyte)EditorGUILayout.IntSlider("Current Layer", CurrentLayer, -5, 5);
		CurrentTool = (EPaintingTool)GUILayout.Toolbar((int)CurrentTool, Enum.GetNames(typeof(EPaintingTool)));
		if (CurrentTool == EPaintingTool.Add)
		{
			var newBrush = (VoxelMaterial)EditorGUILayout.ObjectField("Current Brush", CurrentBrush, typeof(VoxelMaterial), false);
			var dirty = newBrush != CurrentBrush;
			CurrentBrush = newBrush;
			if(CurrentBrush)
			{
				if (dirty || m_cachedBrushEditor == null || m_cachedEditorNeedsRefresh)
				{
					m_cachedBrushEditor = Editor.CreateEditor(CurrentBrush);
					m_cachedEditorNeedsRefresh = false;
				}
				EditorGUILayout.BeginVertical(EditorStyles.foldout);
				m_cachedBrushEditor?.DrawDefaultInspector();
				EditorGUILayout.EndVertical();
			}
		}
		else if (CurrentTool == EPaintingTool.Remove)
		{

		}
		GUI.enabled = true;

		SceneView.RepaintAll();
	}

	void OnSceneGUI()
	{
		Tools.current = Tool.Custom;
		DrawBrushScene();
	}

	void DrawBrushScene()
	{
		if (!Enabled)
		{
			return;
		}
		var collider = Renderer.GetComponent<MeshCollider>();
		var mesh = Renderer.GetComponent<MeshCollider>().sharedMesh;
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
			var p = new Plane(Renderer.transform.up, Renderer.transform.position.y);
			if (!p.Raycast(worldRay, out var planePoint))
			{
				return;
			}
			hitPoint = worldRay.origin + worldRay.direction * planePoint;
			hitNorm = Renderer.transform.up;
		}

		Handles.DrawWireCube(hitPoint, Vector3.one * .02f);
		Handles.DrawLine(hitPoint, hitPoint + hitNorm);

		VoxelCoordinate brushCoord = default;
		Voxel voxel = default;
		switch (CurrentTool)
		{
			case EPaintingTool.Add:
				var scale = VoxelCoordinate.LayerToScale(CurrentLayer);
				brushCoord = VoxelCoordinate.FromVector3(hitPoint + hitNorm * scale / 2f, CurrentLayer);
				break;
			case EPaintingTool.Remove:
			case EPaintingTool.Subdivide:
				var voxelN = Renderer.GetVoxel(triIndex);
				if (voxelN.HasValue)
				{
					voxel = voxelN.Value;
					brushCoord = voxel.Coordinate;					
				}
				else
				{
					return;
				}
				break;
		}

		if(CurrentTool == EPaintingTool.Subdivide)
		{
			foreach (var sub in voxel.Subdivide())
			{
				var subLayerScale = VoxelCoordinate.LayerToScale(sub.Coordinate.Layer);
				var subPos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(sub.Coordinate.ToVector3());
				var subScale = Renderer.transform.localToWorldMatrix.MultiplyVector(subLayerScale * Vector3.one * .4f);
				HandleExtensions.DrawWireCube(subPos, subScale,	Renderer.transform.rotation, Color.magenta);
			}
		}

		var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
		var voxelWorldPos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(brushCoord.ToVector3());
		var voxelScale = Renderer.transform.localToWorldMatrix.MultiplyVector(layerScale * Vector3.one * .51f);

		HandleExtensions.DrawWireCube(voxelWorldPos, voxelScale, Renderer.transform.rotation, Color.cyan);
		Handles.Label(voxelWorldPos, brushCoord.ToString(), EditorStyles.textField);

		if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			switch (CurrentTool)
			{
				case EPaintingTool.Add:
					if(!CurrentBrush)
					{
						return;
					}
					var collScale = Vector3.one * layerScale * .495f;
					var coll = Physics.OverlapBox(voxelWorldPos, collScale, Renderer.transform.rotation);
					if (coll.Any())
					{
						DebugHelper.DrawCube(voxelWorldPos, collScale, Renderer.transform.rotation, Color.red, 5);
						return;
					}

					Renderer.Mesh.Voxels[brushCoord] = new Voxel(brushCoord, CurrentBrush.Data.Copy());
					Renderer.Invalidate();
					break;
				case EPaintingTool.Remove:
					Renderer.Mesh.Voxels.Remove(brushCoord);
					break;
				case EPaintingTool.Subdivide:
					Renderer.Mesh.Voxels.Remove(brushCoord);
					foreach (var sub in voxel.Subdivide())
					{
						Renderer.Mesh.Voxels[sub.Coordinate] = sub;
					}
					break;
			}
			Renderer.Mesh.Hash = System.Guid.NewGuid().ToString();
			EditorUtility.SetDirty(Renderer.Mesh);
			Event.current.Use();
			Debug.LogWarning("Used event");
		}

	}

	public void OnEnable()
	{
		m_cachedBrushEditor = null;
		m_cachedEditorNeedsRefresh = true;
	}
}