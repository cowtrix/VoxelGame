using Common;
using UnityEngine;

public class Checkpoint : TrackedObject<Checkpoint>
{
	public int Level;
	public LevelWin Win;
	
	public float MoveSpeed = 1;
	private Vector3 StartPosition;

	private void Start()
	{
		StartPosition = transform.parent.position;
		if (Level == 0)
		{
			return;
		}
		transform.parent.position += Vector3.down * 100;
	}

	private void Update()
	{
		var chk = GMTKGameManager.Instance.CurrentCheckpoint;
		if (chk && chk.Level < Level)
		{
			return;
		}
		transform.parent.position = Vector3.Lerp(transform.parent.position, StartPosition, Time.deltaTime * MoveSpeed);
	}
}
