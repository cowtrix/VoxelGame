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
	public LayerMask InteractionMask;
	public CameraController CameraController => CameraController.Instance;
	public Interactable FocusedInteractable { get; private set; }
	public int ActionIndex = 0;

	private void Update()
	{
		var cameraForward = CameraController.transform.forward;
		var cameraPos = CameraController.transform.position;

		if (Physics.Raycast(cameraPos, cameraForward, out var interactionHit, 1000, InteractionMask, QueryTriggerInteraction.Collide))
		{
			Debug.DrawLine(cameraPos, interactionHit.point, Color.yellow);
			var interactable = interactionHit.collider.GetComponent<Interactable>() ?? interactionHit.collider.GetComponentInParent<Interactable>();
			if (interactable && interactable.enabled && interactionHit.distance < interactable.InteractionSettings.MaxFocusDistance)
			{
				if(interactable != FocusedInteractable)
				{
					FocusedInteractable?.ExitFocus(this);
					FocusedInteractable = interactable;
					FocusedInteractable.EnterFocus(this);
				}
				return;
			}			
		}
		FocusedInteractable?.ExitFocus(this);
		FocusedInteractable = null;
		Debug.DrawLine(cameraPos, cameraPos + cameraForward * 1000, Color.magenta);

		/*var bestAngle = float.MaxValue;
		var bestDistance = float.MaxValue;
		Interactable newFocused = null;
		foreach (var interactable in Interactables)
		{
			var diffVec = interactable.transform.position - cameraPos;
			var distance = diffVec.magnitude;
			var angle = Vector3.Angle(diffVec.normalized, cameraForward);
			if (angle < bestAngle && distance < bestDistance)
			{
				bestAngle = angle;
				bestDistance = distance;
				newFocused = interactable;
			}
		}
		if (newFocused == FocusedInteractable)
		{
			return;
		}
		FocusedInteractable?.ExitFocus(this);
		FocusedInteractable = newFocused;
		FocusedInteractable?.EnterFocus(this);*/
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started)
		{
			return;
		}

		if (State.EquippedItem)
		{
			State.EquippedItem.UseOn(this, FocusedInteractable);
			return;
		}

		Debug.Log("OnUse: " + FocusedInteractable);
		FocusedInteractable?.Use(this, FocusedInteractable.GetAction(this, ActionIndex));
	}
}
