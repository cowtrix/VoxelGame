using Common;
using Interaction;
using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Actors
{
	[RequireComponent(typeof(ActorState))]
	public class Actor : ExtendedMonoBehaviour, IDialogueActor
	{
		[Serializable]
		public class ActorSettings
		{
			[Header("Interaction")]
			public Vector2 InteractDistance = new Vector2(0, 10);

			[Header("Inventory")]
			public Transform EquippedItemTransform;
			public int EquippedLayer = 1;
		}

		public ActorSettings Settings = new ActorSettings();
		public Interactable FocusedInteractable { get; protected set; }
		public List<Interactable> Interactables { get; private set; } = new List<Interactable>();

		public ActorState State => GetComponent<ActorState>();
		public virtual string DisplayName => name;
		public ILookAdapter LookAdapter { get; protected set; }
		public Transform GetDialogueContainer() => DialogueContainer;
		public Transform DialogueContainer;
		public LayerMask InteractionMask;

		private void Start()
		{
			LookAdapter = gameObject.GetComponentByInterfaceInChildren<ILookAdapter>();
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
				var interactable = interactionHit.collider.GetComponent<Interactable>() ?? interactionHit.collider.GetComponentInParent<Interactable>();
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