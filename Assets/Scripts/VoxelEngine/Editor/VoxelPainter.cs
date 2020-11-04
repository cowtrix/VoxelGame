using DiffMatchPatch;
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
	Select,
	Add,
	Remove,
	Subdivide,
	Paint,
}

[CustomEditor(typeof(VoxelRenderer))]
public class VoxelPainter : Editor
{
	[MenuItem("GameObject/3D Object/Voxel Object")]
	public static void CreateNew()
	{
		var go = new GameObject("New Voxel Object");
		var r = go.AddComponent<VoxelRenderer>();
	}

	Dictionary<EPaintingTool, VoxelPainterTool> m_tools = new Dictionary<EPaintingTool, VoxelPainterTool>
	{
		{ EPaintingTool.Select, new SelectTool() },
		{ EPaintingTool.Add, new AddTool() },
		{ EPaintingTool.Remove, new RemoveTool() },
		{ EPaintingTool.Subdivide, new SubdivideTool() },
		{ EPaintingTool.Paint, new PaintTool() },
	};

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
	public VoxelRenderer Renderer => target as VoxelRenderer;
	public HashSet<VoxelCoordinate> CurrentSelection = new HashSet<VoxelCoordinate>();

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		if(Enabled)
		{
			VoxelManager.Instance.DefaultMaterial.EnableKeyword("ShowGrid");
		}
		else
		{
			VoxelManager.Instance.DefaultMaterial.DisableKeyword("ShowGrid");
		}
		base.OnInspectorGUI();

		if(!Renderer.Mesh)
		{
			EditorGUILayout.HelpBox("Select a Voxel Mesh asset", MessageType.Info);
			return;
		}

		EditorGUILayout.LabelField("Painter", EditorStyles.whiteLargeLabel);
		EditorGUILayout.BeginVertical("Box");
		Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
		GUI.enabled = Enabled;
		var oldTool = CurrentTool;
		CurrentLayer = (sbyte)EditorGUILayout.IntSlider("Current Layer", CurrentLayer, -5, 5);
		var newTool = (EPaintingTool)GUILayout.Toolbar((int)CurrentTool, Enum.GetNames(typeof(EPaintingTool)));
		bool dirty = newTool != CurrentTool;
		CurrentTool = newTool;
		var t = m_tools[CurrentTool];
		if(dirty)
		{
			m_tools[oldTool].OnDisable();
			t.OnEnable();
		}
		EditorGUILayout.BeginVertical("Box");
		if(t.DrawInspectorGUI(this))
		{
			EditorUtility.SetDirty(Renderer.Mesh);
			Renderer.Invalidate(true);
		}
		EditorGUILayout.EndVertical();
		GUI.enabled = true;
		EditorGUILayout.EndVertical();

		SceneView.RepaintAll();
	}

	void OnSceneGUI()
	{
		if(!Enabled)
		{
			return;
		}
		Tools.current = Tool.Custom;
		var tran = Renderer.transform;
		Handles.color = Color.white.WithAlpha(.1f);
		Handles.DrawLine(tran.position - tran.up * 100, tran.position + tran.up * 100);
		Handles.DrawLine(tran.position - tran.right * 100, tran.position + tran.right * 100);
		Handles.DrawLine(tran.position - tran.forward * 100, tran.position + tran.forward * 100);

		var t = m_tools[CurrentTool];
		t.DrawSceneGUI(this, Renderer, Event.current, CurrentLayer);
	}

	public void OnDisable()
	{
		VoxelManager.Instance.DefaultMaterial.DisableKeyword("ShowGrid");
	}
}
