using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	[Range(0,1)]
	public float NormalizedTimeOfDay;
	public int DayCount;

	public GameDateTime CurrentTime => new GameDateTime(DayCount, NormalizedTimeOfDay);

    public SceneReference CurrentScene;
	public PlayerActor Player;

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
		Player = FindObjectOfType<PlayerActor>();
		SceneManager.sceneLoaded += OnSceneLoaded;
		if(!AllScenes().Any(s => s.path == CurrentScene.ScenePath))
		{
			SceneManager.LoadScene(CurrentScene.ScenePath, LoadSceneMode.Additive);
		}
		else
		{
			OnSceneLoaded(default, default);
		}
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode sceneMode)
	{
		var spawn = FindObjectOfType<SpawnPosition>();
		if (spawn)
		{
			Player.transform.position = spawn.transform.position;
			var rb = Player.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	private void Update()
	{
		NormalizedTimeOfDay += Time.deltaTime / 1000;
	}
}
