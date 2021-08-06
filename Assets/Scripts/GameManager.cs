using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	[Range(0,1)]
	public float TimeOfDay;
    public SceneReference CurrentScene;

	public IEnumerable<Scene> AllScenes()
	{
		var count = SceneManager.sceneCount;
		for(var i = 0; i < count; ++i)
		{
			yield return SceneManager.GetSceneAt(i);
		}
	}

	private void Start()
	{
		if(AllScenes().Any(s => s.path == CurrentScene.ScenePath))
		{
			return;
		}
		SceneManager.LoadScene(CurrentScene.ScenePath, LoadSceneMode.Additive);
	}
}
