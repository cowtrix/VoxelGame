using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
	public Image Icon;
    public Text Label;
    public RectTransform FocusTransform;
	public float NoObjectScale = 32;
	public float ObjectScale = 64;

	private Camera Camera => CameraController.GetComponent<Camera>();
	public CameraController CameraController => CameraController.Instance;
	public PlayerInteractionManager InteractionManager;

	private void Update()
	{
		if(InteractionManager.FocusedInteractable)
		{
			FocusTransform.position = Camera.WorldToScreenPoint(InteractionManager.FocusedInteractable.transform.position);
			FocusTransform.sizeDelta = Vector2.one * ObjectScale;
			Icon.enabled = true;
			Icon.sprite = InteractionManager.FocusedInteractable.Icon?.Invoke();
		}
		else
		{
			FocusTransform.position = Camera.WorldToScreenPoint(CameraController.FocusPoint);
			FocusTransform.sizeDelta = Vector2.one * NoObjectScale;
			Icon.sprite = null;
		}
		Icon.enabled = Icon.sprite;
	}
}
