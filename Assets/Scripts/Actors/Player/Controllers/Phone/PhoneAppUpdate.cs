using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class PhoneAppUpdate : ExtendedMonoBehaviour
	{
		public float Height = 30;
		public float ExpandSpeed = 3;
		public float ShowTime = 5;
		public float FadeSpeed = 2;

		public Image AppIcon;
		public TextMeshProUGUI FirstText, SecondText;

		private RectTransform RectTransform => GetComponent<RectTransform>();
		private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

		public void SetData(PhoneApp app, string firstString, string secondString)
		{
			AppIcon.sprite = app.Icon;
			FirstText.text = firstString;
			FirstText.gameObject.SetActive(!string.IsNullOrEmpty(firstString));
			SecondText.text = secondString;
			SecondText.gameObject.SetActive(!string.IsNullOrEmpty(secondString));
			RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, 0);
			StartCoroutine(Fade());
		}

		private IEnumerator Fade()
		{
			var timer = 0f;
			CanvasGroup.alpha = 1;
			while (timer < ShowTime)
			{
				RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, Mathf.Lerp(RectTransform.sizeDelta.y, Height, Time.deltaTime * ExpandSpeed));
				timer += Time.deltaTime;
				yield return null;
			}
			while (CanvasGroup.alpha > 0)
			{
				CanvasGroup.alpha -= FadeSpeed * Time.deltaTime;
				yield return null;
			}
			Destroy(gameObject);
		}
	}
}