using Actors;
using System.Collections.Generic;

namespace Interaction.Items
{
    public class ItemComponentPurchaseable : ItemComponent
    {
        public int Cost = 1;
        public override IEnumerable<ActorAction> AddActions(Actor actor)
        {
            yield return new ActorAction(eActionKey.USE, $"Purchase ({Cost}¢)", Item.gameObject);
        }

        public override bool InterceptAction(Actor actor, ActorAction action)
        {
            if (action.Key == eActionKey.USE && action.State == eActionState.End)
            {
                if (actor.State.TryAdd(eStateKey.Credits, Cost, $"Purchased {Item.DisplayName}"))
                {
                    actor.State.PickupItem(Item, true);
                    Destroy(this);
                }
                return true;
            }
            return base.InterceptAction(actor, action);
        }
    }
}