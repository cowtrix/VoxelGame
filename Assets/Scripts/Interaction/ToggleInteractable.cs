using Actors;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Interaction
{
    public class ToggleInteractable : Interactable
    {
        public override string DisplayName => Name;
        public bool ToggleState
        {
            get
            {
                return m_toggleState;
            }
            set 
            {
                if(m_toggleState == value)
                {
                    return;
                }
                m_toggleState = value;
                if (m_toggleState)
                {
                    ToggleOn.Invoke();
                }
                else
                {
                    ToggleOff.Invoke();
                }
            }
        }
        private bool m_toggleState;

        public string Name;
        public bool StartingToggleState;
        public UnityEvent ToggleOn, ToggleOff;
        public ActorAction TurnOnAction, TurnOffAction;

        protected override void Start()
        {
            m_toggleState = StartingToggleState;
            if (m_toggleState)
            {
                ToggleOn.Invoke();
            }
            else
            {
                ToggleOff.Invoke();
            }
            base.Start();
        }

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
        }
    }
}