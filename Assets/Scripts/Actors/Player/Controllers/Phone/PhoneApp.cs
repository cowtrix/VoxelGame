using Common;
using System;
using UnityEngine;
using Voxul;

public class PhoneApp : ExtendedMonoBehaviour
{
	public PhoneController Phone { get; private set; }
	public Sprite Icon;
	public string AppName;

	public int NotificationCount { get; internal set; }

	private void Awake()
	{
		Phone = transform.GetComponentInAncestors<PhoneController>();
	}

	public virtual void OnOpen(PhoneController phoneController)
	{
		gameObject.SetActive(true);
	}

	public virtual void OnClose(PhoneController phoneController)
	{
		gameObject.SetActive(false);
	}

	protected void SendNotification(string string1, string string2 = null)
	{
		Phone.NotificationManager.CreateNotification(this, string1, string2);
	}
}