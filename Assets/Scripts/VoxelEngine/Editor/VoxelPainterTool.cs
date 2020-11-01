using JetBrains.Annotations;
using MadMaps.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class VoxelPainterTool
{
	private Editor m_cachedBrushEditor;
	private bool m_cachedEditorNeedsRefresh = true;
	protected static VoxelMaterial DefaultMaterial => new VoxelMaterial { Default = new SurfaceData { Albedo = Color.white } };

	protected static VoxelMaterialAsset m_asset;
	protected static VoxelMaterial CurrentBrush
	{
		get
		{
			if (!m_asset)
			{
				m_asset = ScriptableObject.CreateInstance<VoxelMaterialAsset>();
				m_asset.Data = EditorPrefUtility.GetPref("VoxelPainter_Brush", DefaultMaterial);
			}
			return m_asset.Data;
		}
		set
		{
			if (!m_asset)
			{
				m_asset = ScriptableObject.CreateInstance<VoxelMaterialAsset>();
			}
			m_asset.Data = value;
			EditorUtility.SetDirty(m_asset);
		}
	}

	protected abstract EPaintingTool ToolID { get; }

	protected virtual bool GetVoxelDataFromPoint(VoxelPainter voxelPainterTool, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer,
		out List<Voxel> selection, out VoxelCoordinate brushCoord, out EVoxelDirection hitDir)
	{
		hitNorm = renderer.transform.worldToLocalMatrix.MultiplyVector(hitNorm);
		hitDir = VoxelCoordinate.CoordinateToDirection(hitNorm);
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
				HandleExtensions.DrawWireCube(pos, scale, renderer.transform.rotation, new Color(1, 1, 1, .1f));
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
			out var selection, out var brushCoord, out var hitDir))
		{
			return;
		}

		var layerScale = VoxelCoordinate.LayerToScale(brushCoord.Layer);
		var voxelWorldPos = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(brushCoord.ToVector3());
		var voxelScale = layerScale * Vector3.one * .51f;
		voxelScale.Scale(renderer.transform.localToWorldMatrix.GetScale());
		HandleExtensions.DrawWireCube(voxelWorldPos, voxelScale, renderer.transform.rotation, Color.cyan);
		Handles.Label(voxelWorldPos, brushCoord.ToString(), EditorStyles.textField);

		if (DrawSceneGUIInternal(voxelPainter, renderer, currentEvent, selection, brushCoord, hitDir))
		{
			renderer.Mesh.Hash = System.Guid.NewGuid().ToString();
			EditorUtility.SetDirty(renderer.Mesh);
			Event.current.Use();
			Debug.LogWarning("Used event");
		}
	}

	public virtual bool DrawInspectorGUI(VoxelPainter voxelPainter)
	{
		var brushes = AssetDatabase.FindAssets($"t: {nameof(VoxelMaterialAsset)}")
			.Select(b => AssetDatabase.GUIDToAssetPath(b));
		bool dirty = false;
		GUILayout.BeginVertical("Box");
		var selIndex = GUILayout.SelectionGrid(-1, brushes.Select(b => new GUIContent(Path.GetFileNameWithoutExtension(b))).ToArray(), 1);
		if (selIndex >= 0)
		{
			dirty = true;
			CurrentBrush = AssetDatabase.LoadAssetAtPath<VoxelMaterialAsset>(brushes.ElementAt(selIndex)).Data;
			Debug.Log($"Loaded brush from {brushes.ElementAt(selIndex)}");
		}
		GUILayout.EndVertical();
		if (dirty || m_cachedBrushEditor == null || !m_cachedBrushEditor || m_cachedEditorNeedsRefresh || Event.current.alt)
		{
			if (m_cachedBrushEditor)
			{
				UnityEngine.Object.DestroyImmediate(m_cachedBrushEditor);
			}
			m_cachedBrushEditor = Editor.CreateEditor(m_asset);
			m_cachedEditorNeedsRefresh = false;
		}
		m_cachedBrushEditor?.DrawDefaultInspector();
		EditorPrefUtility.SetPref("VoxelPainter_Brush", CurrentBrush);
		return false;
	}

	protected abstract bool DrawSceneGUIInternal(VoxelPainter painter, VoxelRenderer Renderer, Event currentEvent, 
		List<Voxel> selection, VoxelCoordinate brushCoord, EVoxelDirection hitDir);

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}
}