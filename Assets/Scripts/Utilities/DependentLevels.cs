using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Voxul;

public class DependentLevels : ExtendedMonoBehaviour
{
    public List<SceneReference> Levels = new List<SceneReference>();

#if UNITY_EDITOR
    [ContextMenu("Load Dependent Levels")]
    public void LoadInEditor()
    {
        foreach (var s in Levels)
        {
            if(UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(s.ScenePath) == null)
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(s, UnityEditor.SceneManagement.OpenSceneMode.Additive);
        }
    }
#endif

    private void Awake()
    {
        foreach (var s in Levels)
        {
            var loadedScene = SceneManager.GetSceneByPath(s.ScenePath);
            if (loadedScene == null && !loadedScene.isLoaded)
                SceneManager.LoadScene(s, LoadSceneMode.Additive);
        }
    }
}
