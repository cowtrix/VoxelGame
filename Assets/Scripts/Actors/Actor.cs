﻿using Actors.NPC;
using Common;
using Interaction;
using Interaction.Activities;
using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using System.Collections.Generic;
using UnityEngine;

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
        LOOK,
        JUMP,
    }

    public interface IMovementController
    {
        Vector3 CurrentGravity { get; }
        Vector3 MoveDirection { get; set; }
        bool IsGrounded { get; }
        void MoveInput(Vector2 dir);
        void Jump();
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
    public class Actor : SlowUpdater, IDialogueActor
    {
        // Adapters
        public ILookAdapter LookAdapter { get; protected set; }
        public IMovementController MovementController { get; private set; }
        public ActorPerceiver Perceiver { get; protected set; }

        public Transform EquippedItemTransform;

        /// <summary>
        /// The interactable item that the player is currently focused on, i.e. in the crosshairs
        /// </summary>
        public Interactable FocusedInteractable { get; protected set; }
        // Activity the actor is currently engaging in
        public IActivity CurrentActivity { get; protected set; }
        public List<Interactable> Interactables { get; private set; } = new List<Interactable>();
        public Animator Animator { get; private set; }
        public ActorState State => GetComponent<ActorState>();
        public virtual string DisplayName => ActorName;
        public RaycastHit? LastRaycast { get; protected set; }

        public Transform GetDialogueContainer() => DialogueContainer;
        public Transform DialogueContainer;
        public LayerMask InteractionMask;
        public string ActorName = "Unnamed Entity";

        private HashSet<IStatement> m_chatHistory = new HashSet<IStatement>();

        protected virtual void Awake()
        {
            MovementController = gameObject.GetComponentByInterfaceInChildren<IMovementController>();
            LookAdapter = gameObject.GetComponentByInterfaceInChildren<ILookAdapter>();
            Animator = GetComponentInChildren<Animator>(true);
            Perceiver = gameObject.GetOrAddComponent<ActorPerceiver>();
        }

        public virtual void TryStartActivity(IActivity activity)
        {
            if (CurrentActivity == activity)
            {
                return;
            }
            if (CurrentActivity != null && !CurrentActivity.Equals(null))
            {
                TryStopActivity(CurrentActivity);
            }
            CurrentActivity = activity;
            CurrentActivity.OnStartActivity(this);
        }

        public virtual void TryStopActivity(IActivity activity)
        {
            if (CurrentActivity != activity)
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

        protected override int TickOnThread(float dt)
        {
            /*if (LookAdapter == null)
            {
                return 0;
            }
            var cameraForward = LookAdapter.transform.forward;
            var cameraPos = LookAdapter.transform.position;
            var isHit = Physics.Raycast(cameraPos, cameraForward, out var interactionHit, 1000, InteractionMask, QueryTriggerInteraction.Collide);
            LastRaycast = interactionHit;
            if (isHit)
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
                    return 3;
                }
            }

            FocusedInteractable?.ExitFocus(this);
            FocusedInteractable = null;
            Debug.DrawLine(cameraPos, cameraPos + cameraForward * 1000, Color.magenta);*/
            return 0;
        }

        public bool HasSaid(IStatement key) => m_chatHistory.Contains(key);

        public void RecordSaid(IStatement statement) => m_chatHistory.Add(statement);

        public void OnDialogueStarted(DialogueTree dlg, IDialogueUI dialogueUGUI)
        {
            var voice = GetComponentInChildren<ActorVoice>();
            if (voice)
            {
                dialogueUGUI.onWordsSaidEvent.RemoveAllListeners();
                dialogueUGUI.onWordsSaidEvent.AddListener((w) => voice.SayWord(w));
            }
        }
    }
}