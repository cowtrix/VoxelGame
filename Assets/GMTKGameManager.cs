using Common;
using System.Linq;
using UnityEngine;

public class GMTKGameManager : Singleton<GMTKGameManager>
{
	public GameObject Credits;
	public Checkpoint CurrentCheckpoint;
	public void ResetGame()
	{
		Debug.Log("Reset Game");
		foreach (var pickup in Pickup.Instances)
		{
			pickup.gameObject.SetActive(pickup.Level >= CurrentCheckpoint?.Level);
		}
		var player = Player.Instance;
		player.transform.position = CurrentCheckpoint.transform.position;
		player.TargetPosition = CurrentCheckpoint.transform.position;

		ResetPlayerMeshState(player);
	}

	private void ResetPlayerMeshState(Player player)
	{
		var root = player.Renderer.Mesh.Voxels.First();
		player.Renderer.Mesh.Voxels.Clear();
		player.Renderer.Mesh.Voxels.AddSafe(root.Value);
		player.Renderer.Mesh.Invalidate();
		player.Renderer.Invalidate(true, false);
	}

	public void CheckWin()
	{
		var player = Player.Instance;

		if (!CurrentCheckpoint)
		{
			player.TargetPosition = new Vector3(player.TargetPosition.x, 10_000, player.TargetPosition.z);
			player.Renderer.transform.rotation *= Quaternion.Euler(0, Time.deltaTime * 100, 0);
			Credits.gameObject.SetActive(true);
			return;
		}
		Credits.gameObject.SetActive(false);
		if (CurrentCheckpoint.Win && CurrentCheckpoint.Win.CheckWin(player))
		{
			Debug.Log($"Moving to level {CurrentCheckpoint.Win.NextCheckpoint?.Level}");
			CurrentCheckpoint.Win.gameObject.SetActive(false);

			if (CurrentCheckpoint && CurrentCheckpoint.Win)
			{
				var ps = Instantiate(CurrentCheckpoint.Win.ParticleSystem.gameObject).GetComponent<ParticleSystem>();
				ps.gameObject.SetActive(true);
				ps.transform.position = transform.position;
			}

			CurrentCheckpoint = CurrentCheckpoint.Win.NextCheckpoint;
			CurrentCheckpoint?.Win?.gameObject.SetActive(true);

			ResetPlayerMeshState(player);
		}
	}
}
