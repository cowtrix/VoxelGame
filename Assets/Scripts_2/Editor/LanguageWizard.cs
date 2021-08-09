using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LanguageWizard : EditorWindow
{
    public int CharacterLength = 10;
    public string Input, Output;


    [MenuItem("Tools/Generate Random Text")]
    static void CreateWizard()
    {
		GetWindow<LanguageWizard>();
    }

	private void OnGUI()
	{
        titleContent = new GUIContent("Generate Random Text");
		CharacterLength = EditorGUILayout.IntField("Character Length", CharacterLength);
		if (GUILayout.Button("Generate"))
		{
			Output = LanguageUtility.Generate(CharacterLength);
		}

		Input = EditorGUILayout.TextField("Input", Input);
		if (GUILayout.Button("Translate"))
		{
			Output = LanguageUtility.Translate(Input);
		}
		EditorGUILayout.SelectableLabel(Output);
	}
}
