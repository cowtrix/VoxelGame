using Common;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Actors
{
    [Serializable]
    public class ActorPerceptionEvent : UnityEvent<ActorEventData>{}

    public class ActorPerceiver : TrackedObject<ActorPerceiver> 
    {
        public ILookAdapter LookAdapter => gameObject.GetComponentByInterface<ILookAdapter>();
        public ActorPerceptionEvent OnEventPerceived;
        public float MaxPerceptionDistance = 20;

        public static void ReceiveEventGlobal(ActorEventData perceptionEvent)
        {
            foreach(var perceiver in Instances)
            {
                perceiver.ReceiveEvent(perceptionEvent);
            }
        }

        public void ReceiveEvent(ActorEventData perceptionEvent)
        {
            if (!CanPerceive(perceptionEvent))
            {
                return;
            }
            OnEventPerceived.Invoke(perceptionEvent);
        }

        private bool CanPerceive(ActorEventData perceptionEvent) 
        {
            if(Vector3.Distance(transform.position, perceptionEvent.WorldPosition) > MaxPerceptionDistance)
            {
                return false;
            }
            if (perceptionEvent.RequireSight && LookAdapter != null)
            {
                // Check sight
                if (!LookAdapter.CanSee(perceptionEvent.WorldPosition))
                {
                    return false;
                }
            }
            return true;
        }
    }
}