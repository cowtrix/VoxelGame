using System;
using UnityEngine;
using Voxul;

public class PhoneApp : ExtendedMonoBehaviour
{
	public Sprite Icon;
	public string AppName;

	public int NotificationCount { get; internal set; }

	public void OnOpen(PhoneController phoneController)
	{
		gameObject.SetActive(true);
	}

	public void OnClose(PhoneController phoneController)
	{
		gameObject.SetActive(false);
	}
}