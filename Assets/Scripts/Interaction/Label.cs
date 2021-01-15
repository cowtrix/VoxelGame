using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Label : MonoBehaviour
{
	[SerializeField]
    protected string m_text;

	private void Awake()
	{
		var interactable = GetComponent<Interactable>();
		interactable.OnFocusStart.AddListener(OnFocusStart);
		interactable.OnFocusEnd.AddListener(OnFocusEnd);
	}

	public virtual string GetText() => m_text;

	private void OnFocusEnd(PlayerInteractionManager arg0)
	{
		HUDManager.Instance.Label.text = "";
	}

	private void OnFocusStart(PlayerInteractionManager arg0)
	{
		HUDManager.Instance.Label.text = m_text;
	}
}
