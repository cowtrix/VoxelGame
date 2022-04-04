using Actors;
using UnityEngine;

public class ActionStringTest : MonoBehaviour
{
	public eActionKey Key;
	private void OnDrawGizmos()
	{
		Debug.Log(CameraController.Instance.Input.GetControlNameForAction(Key));
	}
}
