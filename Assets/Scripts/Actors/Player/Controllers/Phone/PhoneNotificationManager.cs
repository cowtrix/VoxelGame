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

		private void Update()
		{
			CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, Phone.IsOpen ? 0 : 1, Time.deltaTime * FadeSpeed);
		}

		public void CreateNotification(PhoneApp app, string string1, string string2 = null)
		{
			Debug.Log($"Creating new phone notification from {app.AppName}: {string1}\t{string2}");

			var newNotification = Instantiate(PhoneUpdatePrefab.gameObject).GetComponent<PhoneAppUpdate>();
			newNotification.transform.SetParent(transform);
			newNotification.transform.localPosition = Vector3.zero;
			newNotification.transform.localScale = Vector3.one;
			newNotification.transform.localRotation = Quaternion.identity;

			newNotification.SetData(app, string1, string2);
		}
	}
}