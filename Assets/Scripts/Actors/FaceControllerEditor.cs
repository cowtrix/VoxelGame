using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TransformAnimationController))]
public class FaceControllerEditor : Editor
{
	string m_newExpName = "";

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var fc = target as TransformAnimationController;
		
		foreach (var exp in fc.Expressions)
		{
			bool isSelected = exp.Name == fc.TargetExpression;
			EditorGUILayout.BeginHorizontal();
			GUI.color = isSelected ? Color.green : Color.white;
			EditorGUILayout.LabelField(exp.Name);
			if(GUILayout.Button("Set"))
			{
				fc.TargetExpression = exp.Name;
				if (!EditorApplication.isPlaying)
				{
					fc.SetExpression(exp, 1);
				}
				EditorUtility.SetDirty(fc);
			}
			if (GUILayout.Button("Record"))
			{
				fc.RecordCurrent(exp.Name);
				EditorUtility.SetDirty(fc);
			}
			if (GUILayout.Button("Delete"))
			{
				fc.Expressions.Remove(exp);
				EditorUtility.SetDirty(fc);
				EditorGUIUtility.ExitGUI();
				break;
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		m_newExpName = EditorGUILayout.TextField(m_newExpName);
		GUI.enabled = !string.IsNullOrEmpty(m_newExpName);
		if (GUILayout.Button("Record New Expression"))
		{
			fc.RecordCurrent(m_newExpName);
			EditorUtility.SetDirty(fc);
		}
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();

		if (!string.IsNullOrEmpty(fc.TargetExpression))
		{
			EditorGUILayout.HelpBox($"Transitioning to {fc.TargetExpression}", MessageType.Info);
		}
	}
}
#endif