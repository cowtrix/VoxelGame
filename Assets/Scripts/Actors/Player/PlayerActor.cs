using Common;
using Interaction;
using Interaction.Activities;
using NodeCanvas.DialogueTrees;
using Phone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Actors
{
	public static class ActorExtensions
	{
		static eActionKey[] m_keys = Enum.GetValues(typeof(eActionKey)).Cast<eActionKey>().ToArray();
		public static eActionKey GetActionKey(this InputAction.CallbackContext cntxt)
		{
			foreach(var k in m_keys)
			{
				if(string.Equals(k.ToString(), cntxt.action.name, StringComparison.OrdinalIgnoreCase))
				{
					return k;
				}
			}
			throw new Exception($"Input action with no key: {cntxt.action.name}");
		}

		public static string GetControlNameForAction(this PlayerInput input, eActionKey key)
		{
			foreach(var action in input.actions)
			{
				if(string.Equals(action.name, key.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return action.GetBindingDisplayString();
				}
			}
			throw new Exception($"Couldn't find a control name for {key}");
		}
	}

	public class PlayerActor : Actor
	{
		public CameraController CameraController => CameraController.Instance;
		public PhoneController Phone => GetComponentInChildren<PhoneController>(true);

		public void OnActionExecuted(InputAction.CallbackContext cntxt)
		{
			if (!cntxt.started || CameraController.LockCameraLook)
			{
				return;
			}
			var action = new ActorAction { Key = cntxt.GetActionKey(), Context = cntxt.valueType == typeof(Vector2) ? cntxt.ReadValue<Vector2>() : default };
				if(State.EquippedItem != null)
			{
				State.EquippedItem.ExecuteAction(this, action);
				return;
			}
			if (FocusedInteractable is FocusableInteractable focusable && focusable.Actor == this)
			{
				focusable.ExecuteAction(this, action);
				return;
			}
			if (FocusedInteractable && State.EquippedItem == null)
			{
				if(State.EquippedItem == null)
				{
					FocusedInteractable.ExecuteAction(this, action);
				}
				else
				{
					State.EquippedItem.UseOn(this, FocusedInteractable.gameObject);
				}
				return;
			}
		}

		public void EnablePhone()
		{
			Phone.gameObject.SetActive(true);
		}
	}
}