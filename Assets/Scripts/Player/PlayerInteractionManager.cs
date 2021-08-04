using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : Singleton<PlayerInteractionManager>
{
	public CameraController CameraController => CameraController.Instance;
	public Interactable FocusedInteractable;

	public override void Awake()
	{
		base.Awake();
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started)
		{
			return;
		}
		Debug.Log("PlayerInteractionManager:Fire");
	}
}
