using Actors;
using Interaction.Activities;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class NPCInteractable : Activity
    {
        public override Vector3? LookPositionOverride => null;
        public override Quaternion? LookDirectionOverride => null;

        public NPCActor Self;
        public override string DisplayName => Self.DisplayName;

        protected override void Start()
        {
            base.Start();
            Self.Dialogue.OnFinished.AddListener(() => Actor?.TryStopActivity(this));
        }

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (Actor == context)
            {
                yield return new ActorAction(eActionKey.EXIT, "Stop Talking", gameObject);
            }
            else
            {
                if (Self.CanTalkTo(context))
                {
                    yield return new ActorAction(eActionKey.USE, "Talk", gameObject);
                }
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if(action.Key == eActionKey.USE && action.State == eActionState.End && CanUse(actor))
            {
                Self.InteractWithActor(actor);
            }
        }

        public override void OnStartActivity(Actor actor)
        {
            /*if (!Actor && Self.CanTalkTo(actor))
            {
                Self.InteractWithActor(actor);
            }*/
            //base.OnStartActivity(actor);
        }

        public override bool CanUse(Actor context) => Self.CanTalkTo(context);

        public override void OnStopActivity(Actor actor)
        {
            
            base.OnStopActivity(actor);
        }
    }
}