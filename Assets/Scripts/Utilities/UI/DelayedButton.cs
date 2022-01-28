using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class DelayedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onClicked;
    public float PressTime = 1f;

	public Slider Progress;

	private bool m_isPressed;
    private float m_pressedTime;

	private void Update()
	{
		var f = Mathf.Clamp01(m_pressedTime / PressTime);
		Progress.value = f;
		if (!m_isPressed)
		{
			m_pressedTime = 0;
			return;
		}
		m_pressedTime += Time.deltaTime;
		if(f >= 1)
		{
			onClicked.Invoke();
			m_pressedTime = 0;
			m_isPressed = false;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		m_isPressed = true;
		m_pressedTime = 0;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		m_isPressed = false;
	}
}
