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
	private Editor m_cachedBrushEditor;
	private bool m_cachedEditorNeedsRefresh = true;

	[SerializeField]
	private VoxelMaterialAsset m_asset;
	public VoxelMaterial CurrentBrush
	{
		get
		{
			if (!m_asset)
			{
				m_asset = ScriptableObject.CreateInstance<VoxelMaterialAsset>();
				m_asset.Data = DefaultMaterial;
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

	public override void OnEnable()
	{
		CurrentBrush = EditorPrefUtility.GetPref("VoxelPainter_Brush", DefaultMaterial);
	}

	protected override EPaintingTool ToolID => EPaintingTool.Add;

	protected override bool GetVoxelDataFromPoint(VoxelPainter painter, VoxelRenderer renderer, Vector3 hitPoint, Vector3 hitNorm, int triIndex, sbyte layer,
		out List<Voxel> selection, out VoxelCoordinate brushCoord)
	{
		if(Event.current.alt)
		{
			return base.GetVoxelDataFromPoint(painter, renderer, hitPoint, hitNorm, triIndex, layer, out selection, out brushCoord);
		}
		hitPoint = renderer.transform.worldToLocalMatrix.MultiplyPoint3x4(hitPoint);
		hitNorm = renderer.transform.worldToLocalMatrix.MultiplyVector(hitNorm);
		var scale = VoxelCoordinate.LayerToScale(layer);
		brushCoord = VoxelCoordinate.FromVector3(hitPoint + hitNorm * scale / 2f, layer);
		selection = null;
		return true;
	}

	private VoxelMaterial DefaultMaterial => new VoxelMaterial { Default = new SurfaceData { Albedo = Color.gray } };

	public override void DrawInspectorGUI(VoxelPainter voxelPainter)
	{
		if (Event.current.alt)
		{
			EditorGUILayout.HelpBox("Sampling. Click anywhere to get data.", MessageType.Info);
		}

		var brushes = AssetDatabase.FindAssets($"t: {nameof(VoxelMaterialAsset)}")
			.Select(b => AssetDatabase.GUIDToAssetPath(b));
		bool dirty = false;
		GUILayout.BeginVertical("Box");
		var selIndex = GUILayout.SelectionGrid(-1, brushes.Select(b => new GUIContent(Path.GetFileNameWithoutExtension(b))).ToArray(), 1);
		if (selIndex >= 0)
		{
			dirty = true;
			CurrentBrush = AssetDatabase.LoadAssetAtPath<VoxelMaterialAsset>(brushes.ElementAt(selIndex)).Data;
		}
		GUILayout.EndVertical();

		if (dirty || m_cachedBrushEditor == null || !m_cachedBrushEditor || m_cachedEditorNeedsRefresh)
		{
			m_cachedBrushEditor = Editor.CreateEditor(m_asset);
			m_cachedEditorNeedsRefresh = false;
		} 
		m_cachedBrushEditor?.DrawDefaultInspector();
		EditorPrefUtility.SetPref("VoxelPainter_Brush", CurrentBrush);
	}

	protected override bool DrawSceneGUIInternal(VoxelPainter voxelPainter, VoxelRenderer renderer, Event currentEvent, List<Voxel> selection, VoxelCoordinate brushCoord)
	{
		if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
		{
			if (Event.current.alt)
			{
				var vox = selection.First();
				CurrentBrush = vox.Material.Copy();
				return false;
			}

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
			renderer.Mesh.Voxels[brushCoord] = new Voxel(brushCoord, CurrentBrush.Copy());
			renderer.Invalidate();
			return true;
		}
		return false;
	}
}