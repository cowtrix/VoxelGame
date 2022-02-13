using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ReplaceByName : ScriptableWizard
{
    public string Pattern;
    public GameObject Replacement;

    [MenuItem("GameObject/Replace By Name")]
    static void CreateWizard()
    {
        DisplayWizard<ReplaceByName>("Replace By Name", "Replace", "Preview");
    }

    IEnumerable<GameObject> GetTransforms(string rgx)
	{
        return FindObjectsOfType<GameObject>()
            .Where(g => Regex.IsMatch(g.name, Pattern));
	}

    void OnWizardCreate()
    {
		if (!Replacement)
		{
            return;
		}
        var allMatches = GetTransforms(Pattern).ToArray();
        if(!EditorUtility.DisplayDialog($"Replace {allMatches.Length} objects?", "", "Yes", "No"))
		{
            return;
		}

        var trash = new GameObject("Trash");

        Undo.RecordObjects(allMatches, $"ReplaceByName_{Pattern}");
		for (int i = 0; i < allMatches.Length; i++)
		{
			GameObject go = allMatches[i];
			var pos = go.transform.localPosition;
            var rot = go.transform.localRotation;
            var scale = go.transform.localScale;
            var parent = go.transform.parent;

            var newObj = PrefabUtility.InstantiatePrefab(Replacement) as GameObject;
            newObj.transform.SetParent(parent);
            newObj.transform.localPosition = pos;
            newObj.transform.localRotation = rot;
            newObj.transform.localScale = scale;
		}
    }

    void OnWizardOtherButton()
    {
        var allMatches = GetTransforms(Pattern).ToArray();
        Debug.Log($"{allMatches.Length} Objects matched:\n{string.Join('\n', allMatches.Select(go => go.name))}");
        foreach(var go in allMatches)
            EditorGUIUtility.PingObject(go);
    }
}
