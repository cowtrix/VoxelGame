using UnityEngine;
using UnityEngine.Events;

public class ResetLevel : MonoBehaviour
{
	public Bounds Bounds;
	public int Level;

	private void Update()
	{
		var p = transform.worldToLocalMatrix.MultiplyPoint3x4(Player.Instance.transform.position);
		if (Bounds.Contains(p))
		{
			GMTKGameManager.Instance.ResetGame();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}