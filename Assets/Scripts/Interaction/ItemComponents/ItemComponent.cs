using Actors;
using Common;
using System.Collections;
using System.Collections.Generic;
using Voxul;

namespace Interaction.Items
{
    public abstract class ItemComponent : ExtendedMonoBehaviour
    {
        public IItem Item => gameObject.GetComponentByInterface<IItem>();

        public virtual IEnumerable<ActorAction> AddActions(Actor actor)
        {
            yield break;
        }

        public virtual bool InterceptAction(Actor actor, ActorAction action)
        {
            return false;
        }
    }
}