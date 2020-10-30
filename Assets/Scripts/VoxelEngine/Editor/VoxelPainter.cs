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
	private VoxelRenderer Renderer => target as VoxelRenderer;

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

		EditorGUILayout.LabelField("Painter", EditorStyles.whiteLargeLabel);
		EditorGUILayout.BeginVertical("Box");
		Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
		GUI.enabled = Enabled;
		CurrentLayer = (sbyte)EditorGUILayout.IntSlider("Current Layer", CurrentLayer, -5, 5);
		CurrentTool = (EPaintingTool)GUILayout.Toolbar((int)CurrentTool, Enum.GetNames(typeof(EPaintingTool)));
		var t = m_tools[CurrentTool];
		EditorGUILayout.BeginVertical("Box");
		t.DrawInspectorGUI(this);
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
		var t = m_tools[CurrentTool];
		t.DrawSceneGUI(this, Renderer, Event.current, CurrentLayer);
	}

	public void OnEnable()
	{
		foreach(var t in m_tools)
		{
			t.Value.OnEnable();
		}
	}

	public void OnDisable()
	{
		VoxelManager.Instance.DefaultMaterial.DisableKeyword("ShowGrid");
	}
}
