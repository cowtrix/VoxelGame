using Actors;
using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public abstract class PhoneApp : TrackedObject<PhoneApp>, IStateProvider
	{
		private const float MoveSpeed = 8;
		private static Vector2 ClosedPosition = new Vector2(0, -290);
		private static Vector2 OpenPosition = new Vector2(0, -16);

		public Sprite Icon;
		public string AppName;

		public int NotificationCount { get; internal set; }
		public PhoneController Phone { get; private set; }
		protected RectTransform RectTransform => GetComponent<RectTransform>();
		private Coroutine m_moveCoroutine;

		public virtual void Initialise(PhoneController controller)
		{
			Phone = controller;
		}

		public virtual void OnOpen(PhoneController phoneController)
		{
			if (m_moveCoroutine != null) {
				Phone.StopCoroutine(m_moveCoroutine);
			}
			RectTransform.anchoredPosition = ClosedPosition;
			m_moveCoroutine = Phone.StartCoroutine(MoveAndSetEnabled(RectTransform, OpenPosition, true));
			NotificationCount = 0;
		}

		public virtual void OnClose(PhoneController phoneController)
		{
			if (m_moveCoroutine != null)
			{
				Phone.StopCoroutine(m_moveCoroutine);
			}
			m_moveCoroutine = Phone.StartCoroutine(MoveAndSetEnabled(RectTransform, ClosedPosition, false));
			NotificationCount = 0;
		}

		IEnumerator MoveAndSetEnabled(RectTransform rt, Vector2 targetPos, bool enabled)
		{
			if (enabled)
			{
				gameObject.SetActive(enabled);
			}
			while((rt.anchoredPosition - targetPos).magnitude > 0.1f)
			{
				rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, Time.deltaTime * MoveSpeed);
				yield return null;
			}
			gameObject.SetActive(enabled);
			m_moveCoroutine = null;
		}

		protected void SendNotification(string string1, string string2 = null)
		{
			Phone.NotificationManager.CreateNotification(this, string1, string2);
			NotificationCount++;
		}

		public virtual string GetSaveData()
		{
			return null;
		}

		public virtual void LoadSaveData(string data)
		{
		}
	}
}