using Common;
using NodeCanvas.DialogueTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerActor : Actor
{
	public CameraController CameraController => CameraController.Instance;
	public PhoneController Phone => GetComponentInChildren<PhoneController>(true);
	public int ActionIndex = 0;

	protected override void Update()
	{
		if (FocusedInteractable != null
			&& FocusedInteractable is FocusableInteractable
			&& CameraController.Proxy == FocusedInteractable as ICameraControllerProxy)
		{
			return;
		}
		base.Update();
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started || CameraController.LockCameraLook)
		{
			return;
		}

		if (State.EquippedItem)
		{
			Debug.Log("OnFire: " + FocusedInteractable);
			State.EquippedItem.UseOn(this, FocusedInteractable);
			return;
		}
		else if(FocusedInteractable is FocusableInteractable focusable && focusable.Actor == this)
		{
			focusable.Fire(this);
		}
	}

	public void OnUse(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started || CameraController.LockCameraLook)
		{
			return;
		}

		if (FocusedInteractable is FocusableInteractable focusable && focusable.Actor == this)
		{
			Debug.Log("OnStopUse: " + FocusedInteractable);
			focusable.Use(this, Interactable.STOP_USE);
			return;
		}

		Debug.Log("OnUse: " + FocusedInteractable);
		FocusedInteractable?.Use(this, FocusedInteractable.GetAction(this, ActionIndex));
	}

	public void EnablePhone()
	{
		Phone.gameObject.SetActive(true);
	}
}
