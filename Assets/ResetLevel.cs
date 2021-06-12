using UnityEngine;
using UnityEngine.Events;

public class ResetLevel : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var p = other.GetComponentInParent<Player>();
		if (!p)
		{
			return;
		}
		GMTKGameManager.Instance.ResetGame();
	}
}