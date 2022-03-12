using Common;
using Interaction;
using Interaction.Activities;
using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

namespace Actors
{
	public enum eActionKey
	{
		USE,
		EXIT,
		FIRE,
		NEXT,
		PREV,
		EQUIP,
		MOVE,
	}

	public interface IMovementController
	{
		void Move(Vector2 dir);
	}

	public enum eStateKey
	{
		Credits,
		Fuel,
	}

	public struct StateUpdate<T>
	{
		public eStateKey StateKey;
		public T Delta;
		public T Value;
		public string Description;
		public bool Success;

		public StateUpdate(eStateKey key, string desc, T val, T delta, bool success)
		{
			StateKey = key;
			Value = val;
			Delta = delta;
			Description = desc;
			Success = success;
		}
	}

	[RequireComponent(typeof(ActorState))]
	public class Actor : ExtendedMonoBehaviour, IDialogueActor
	{
		// Adapters
		public ILookAdapter LookAdapter { get; protected set; }
		public IMovementController MovementController { get; private set; }

		public Transform EquippedItemTransform;

		/// <summary>
		/// The interactable item that the player is currently focused on, i.e. in the crosshairs
		/// </summary>
		public Interactable FocusedInteractable { get; protected set; }
		// Activity the actor is currently engaging in
		public Activity CurrentActivity { get; protected set; }
		public List<Interactable> Interactables { get; private set; } = new List<Interactable>();
		public Animator Animator { get; private set; }
		public ActorState State => GetComponent<ActorState>();
		public virtual string DisplayName => ActorName;

		public Transform GetDialogueContainer() => DialogueContainer;
		public Transform DialogueContainer;
		public LayerMask InteractionMask;
		public string ActorName = "Unnamed Entity";

		private void Awake()
		{
			MovementController = gameObject.GetComponentByInterfaceInChildren<IMovementController>();
			LookAdapter = gameObject.GetComponentByInterfaceInChildren<ILookAdapter>();
			Animator = GetComponentInChildren<Animator>();
		}

		protected virtual void Update()
		{
			if (LookAdapter == null)
			{
				return;
			}
			var cameraForward = LookAdapter.transform.forward;
			var cameraPos = LookAdapter.transform.position;

			if (Physics.Raycast(cameraPos, cameraForward, out var interactionHit, 1000, InteractionMask, QueryTriggerInteraction.Collide))
			{
				Debug.DrawLine(cameraPos, interactionHit.point, Color.yellow);
				var interactable = interactionHit.collider.GetComponent<Interactable>() ?? interactionHit.collider.GetComponent<InteractionForwarder>()?.Interactable;
				if (interactable && interactable.enabled && interactionHit.distance < interactable.InteractionSettings.MaxFocusDistance)
				{
					if (interactable != FocusedInteractable)
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
		}

		public virtual void TryStartActivity(Activity activity)
		{
			if(CurrentActivity == activity)
			{
				return;
			}
			if (CurrentActivity)
			{
				TryStopActivity(CurrentActivity);
			}
			CurrentActivity = activity;
			CurrentActivity.OnStartActivity(this);
			CurrentActivity = activity;
		}

		public virtual void TryStopActivity(Activity activity)
		{
			if(CurrentActivity != activity)
			{
				return;
			}
			CurrentActivity.OnStopActivity(this);
			CurrentActivity = null;
		}

		private void OnTriggerEnter(Collider other)
		{
			var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
			if (!interactable || Interactables.Contains(interactable))
			{
				return;
			}
			Interactables.Add(interactable);
			interactable.EnterAttention(this);
		}

		private void OnTriggerExit(Collider other)
		{
			var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
			if (!interactable)
			{
				return;
			}
			Interactables.Remove(interactable);
			interactable.ExitAttention(this);
		}
	}
}