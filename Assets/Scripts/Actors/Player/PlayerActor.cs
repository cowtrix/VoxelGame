using Common;
using Interaction;
using Interaction.Activities;
using NodeCanvas.DialogueTrees;
using Phone;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Actors
{
	public class PlayerActor : Actor
	{
		public CameraController CameraController => CameraController.Instance;
		public PhoneController Phone => GetComponentInChildren<PhoneController>(true);

		public void OnActionExecuted(InputAction.CallbackContext cntxt)
		{
			// Construct action
			var action = new ActorAction
			{
				Key = cntxt.GetActionKey(),
				State = cntxt.started ? eActionState.Start : cntxt.canceled ? eActionState.End : eActionState.Tick,
				Context = cntxt.valueType == typeof(Vector2) ? cntxt.ReadValue<Vector2>() : default 
			};

			//Debug.Log($"Action: {action} {action.State} {action.Context}");

			// First priority is if we have an active activity
			if (CurrentActivity)
			{
				CurrentActivity.ReceiveAction(this, action);
				return;
			}

			if(action.Key == eActionKey.MOVE)
			{
				// Always send mvoement to movement controller otherwise
				MovementController.Move(action.Context);
				return;
			}

			// If we have an equipped item, send it the action
			if (State.EquippedItem != null 
				&& State.EquippedItem.GetActions(this).Any(a => a.Key == action.Key))
			{
				State.EquippedItem.ReceiveAction(this, action);
				return;
			}

			// Otherwise, execute the action on the focused object
			if (FocusedInteractable)
			{
				var actions = FocusedInteractable.GetActions(this);
				if(actions.Any(a => a.Key == action.Key))
				{
					FocusedInteractable.ReceiveAction(this, action);
					return;
				}
			}
		}

		public void EnablePhone()
		{
			Phone.gameObject.SetActive(true);
		}
	}
}