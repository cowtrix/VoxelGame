using Actors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class GameManager : Singleton<GameManager>
{
	public class GameState
	{
		public float NormalizedTimeOfDay;
		public int DayCount;
		public List<JObject> EntityData;
	}

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

			Player.CameraController.LookAt(spawn.transform.forward);
		}
	}

	private void Update()
	{
		NormalizedTimeOfDay += Time.deltaTime / 1000;
		while(NormalizedTimeOfDay > 1)
		{
			NormalizedTimeOfDay--;
		}
	}

	[ContextMenu("Save Game")]
	public void SaveGame()
	{
		var state = new GameState
		{
			NormalizedTimeOfDay = NormalizedTimeOfDay,
			DayCount = DayCount,
			EntityData = StateContainer.Instances.Select(c => c.GetSaveData()).ToList()
		};
	}

	[ContextMenu("Load Game")]
	public void LoadGame(GameState saveData)
	{
		NormalizedTimeOfDay = saveData.NormalizedTimeOfDay;
		DayCount = saveData.DayCount;
		foreach(var entityData in saveData.EntityData)
		{
			var guid = entityData[nameof(StateContainer.GUID)].Value<string>();
			var statecontainer = StateContainer.Instances.FirstOrDefault(a => a.GUID == guid);
			if(statecontainer == null)
			{
				Debug.LogWarning($"Failed to load state for entity {guid}");
				continue;
			}
			statecontainer.LoadSaveData(entityData);
		}
	}
}
