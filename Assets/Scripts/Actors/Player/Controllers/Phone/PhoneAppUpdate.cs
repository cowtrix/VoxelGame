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
        public PhoneApp App;
        public TextMeshProUGUI FirstText, SecondText;

        private RectTransform RectTransform => GetComponent<RectTransform>();
        private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
        private Coroutine m_fadeCoroutine;
        private PhoneNotificationManager m_manager;

        public void SetData(PhoneNotificationManager manager, PhoneApp app, string firstString, string secondString)
        {
            m_manager = manager;
            App = app;
            AppIcon.sprite = app.Icon;
            FirstText.text = firstString;
            FirstText.gameObject.SetActive(!string.IsNullOrEmpty(firstString));
            SecondText.text = secondString;
            SecondText.gameObject.SetActive(!string.IsNullOrEmpty(secondString));
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, 0);
            if(m_fadeCoroutine != null)
            {
                StopCoroutine(m_fadeCoroutine);
            }
            m_fadeCoroutine = StartCoroutine(Fade());
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
            m_manager.OnUpdateDestroyed(this);
            Destroy(gameObject);
        }
    }
}