using Actors;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Interaction
{
    public class ToggleInteractable : Interactable
    {
        public override string DisplayName => Name;
        public bool ToggleState { get; set; }

        public string Name;
        public UnityEvent ToggleOn, ToggleOff;
        public ActorAction TurnOnAction, TurnOffAction;

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (!CanUse(context))
            {
                yield break;
            }
            if (ToggleState)
            {
                yield return TurnOffAction;
            }
            else
            {
                yield return TurnOnAction;
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if (!CanUse(actor))
            {
                return;
            }
            ToggleState = !ToggleState;
            if (ToggleState)
            {
                ToggleOn.Invoke();
            }
            else
            {
                ToggleOff.Invoke();
            }
        }
    }
}