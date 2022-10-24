using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Phone
{
	public class PhoneNotificationManager : ExtendedMonoBehaviour
	{
		public PhoneAppUpdate PhoneUpdatePrefab;
		public PhoneController Phone;

		public float FadeSpeed = 3;
		private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
		private Dictionary<PhoneApp, PhoneAppUpdate> m_updates = new Dictionary<PhoneApp, PhoneAppUpdate>();

		private void Update()
		{
			CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, Phone.IsOpen ? 0 : 1, Time.deltaTime * FadeSpeed);
		}

		public void OnUpdateDestroyed(PhoneAppUpdate notification)
		{
			m_updates.Remove(notification.App);
		}

		public void CreateNotification(PhoneApp app, string string1, string string2 = null)
		{
			Debug.Log($"Creating new phone notification from {app.AppName}: {string1}\t{string2}");

			if(m_updates.TryGetValue(app, out var notification))
			{
                notification = Instantiate(PhoneUpdatePrefab.gameObject).GetComponent<PhoneAppUpdate>();
                notification.transform.SetParent(transform);
                notification.transform.localPosition = Vector3.zero;
                notification.transform.localScale = Vector3.one;
                notification.transform.localRotation = Quaternion.identity;
				m_updates[app] = notification;
            }

            notification.SetData(this, app, string1, string2);
		}
	}
}