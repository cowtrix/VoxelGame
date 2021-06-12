using UnityEngine;

public class GMTKGameManager : Singleton<GMTKGameManager>
{
	public Checkpoint CurrentCheckpoint;
	public void ResetGame()
	{
		Debug.Log("Reset Game");
		foreach(var pickup in Pickup.Instances)
		{
			pickup.gameObject.SetActive(pickup.Level >= CurrentCheckpoint.Level);
		}
		var player = Player.Instance;
		player.transform.position = CurrentCheckpoint.transform.position;
		player.TargetPosition = CurrentCheckpoint.transform.position;
		player.transform.rotation = Quaternion.identity;
		player.TargetRotation = CurrentCheckpoint.transform.rotation;
	}
	public void CheckWin()
	{
		if (CurrentCheckpoint.Win.CheckWin(Player.Instance))
		{
			CurrentCheckpoint = CurrentCheckpoint.Win.NextCheckpoint;
		}
	}
}
