using Actors;
using Common;
using System.Collections;
using System.Collections.Generic;
using Voxul;

namespace Interaction.Items
{
    public abstract class ItemComponent : ExtendedMonoBehaviour, IItemComponent
    {
        public IItem Item => gameObject.GetComponentByInterface<IItem>();

        public virtual IEnumerable<ActorAction> GetActions(Actor actor)
        {
            yield break;
        }

        public virtual void OnDrop(Actor actor)
        {
        }

        public virtual void OnPickup(Actor actor)
        {
        }

        public virtual bool ReceiveAction(Actor actor, ActorAction action)
        {
            return false;
        }
    }
}