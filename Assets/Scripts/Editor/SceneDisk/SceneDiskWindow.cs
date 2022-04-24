using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SceneDisk
{
    public class SceneDiskWindow : EditorWindow
    {
        [MenuItem("Tools/Scene Disk")]
        public static void OpenWindow()
        {
            var w = GetWindow<SceneDiskWindow>();
            w.titleContent = new GUIContent(EditorGUIUtility.IconContent("scenevis_scene_hover", "Scene Data"));
        }

        public SceneAnalysisCapture CurrentCapture { get; private set; }
        private Vector2 m_scroll;

        private void OnGUI()
        {
            if (GUILayout.Button("Analyse Open Scenes"))
            {
                CurrentCapture = SceneDisk.AnalyzeScenes();
            }
            if(CurrentCapture == null)
            {
                return;
            }
            m_scroll = EditorGUILayout.BeginScrollView(m_scroll, "Box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.indentLevel = 0;
            foreach(var scene in CurrentCapture.Scenes)
            {
                DrawScene(scene);
            }
            foreach (var so in CurrentCapture.ScriptableObjects)
            {
                DrawScriptableObjectInfo(so);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawScene(SceneAnalysisCapture.SceneData scene)
        {
            scene.Expanded = EditorGUILayout.Foldout(scene.Expanded, new GUIContent($"{scene.Scene.name} ({scene.GameObjects.Sum(go => go.TotalMessageCount)})"));
            if (!scene.Expanded)
            {
                return;
            }
            foreach(var obj in scene.GameObjects)
            {
                if(obj.TotalMessageCount > 0)
                {
                    DrawGameObjectInfo(obj);
                }
            }
        }

        private static int IndentLevel = 1;
        private void DrawGameObjectInfo(GameObjectData obj)
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("", GUILayout.Width(IndentLevel * 20));
            if(GUILayout.Button(obj.Expanded ? EditorGUIUtility.IconContent("d_Toolbar Minus") : EditorGUIUtility.IconContent("d_Toolbar Plus"), GUILayout.Width(20)))
            {
                obj.Expanded = !obj.Expanded;
            }
            EditorGUILayout.ObjectField("", obj.GameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true));
            GUILayout.Label($"({obj.TotalMessageCount})", GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (!obj.Expanded)
            {
                return;
            }
            IndentLevel++;
            foreach (var message in obj.Messages)
            {
                EditorGUILayout.SelectableLabel(message.Message);
            }
            foreach(var child in obj.Children)
            {
                if(child.TotalMessageCount > 0)
                {
                    DrawGameObjectInfo(child);
                }
            }
            IndentLevel--;
        }

        private void DrawScriptableObjectInfo(ScriptableObjectData obj)
        {
            obj.Expanded = EditorGUILayout.Foldout(obj.Expanded, new GUIContent(obj.ScriptableObject.name));
            if (!obj.Expanded)
            {
                return;
            }
            EditorGUI.indentLevel++;
            EditorGUILayout.ObjectField("", obj.ScriptableObject, typeof(GameObject), true);
            foreach (var message in obj.Messages)
            {
                EditorGUILayout.SelectableLabel(message.Message);
            }
            EditorGUI.indentLevel--;
        }
    }
}
