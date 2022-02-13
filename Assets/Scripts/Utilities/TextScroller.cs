using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroller : MonoBehaviour
{
    [Multiline]
    public string Content;
	public string Divider;
	public float Speed = 1;
	protected Text Text => GetComponent<Text>();
	public string m_alienText;
	private int m_index;

	private void Awake()
	{
		m_alienText = LanguageUtility.Translate(Content);
		StartCoroutine(Tick());
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			Text.text = m_alienText.SafeSubstring(m_index) + Divider + m_alienText.SafeSubstring(0, m_index - 1);
			m_index++;
			if (m_index >= m_alienText.Length)
			{
				m_index = 0;
			}
			yield return new WaitForSeconds(Speed);
		}
	}
}
