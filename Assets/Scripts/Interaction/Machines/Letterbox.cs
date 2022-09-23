using Actors;
using System;
using System.Collections.Generic;
using Voxul.Utilities;

namespace Interaction.Items
{
    public class Letterbox : Interactable
    {
        public string Address;
        public override string DisplayName => $"Letter Box: {Address}";

        private bool ItemIsLetter(IItem item) => item.Category.HasFlag(eItemCategory.Mail);

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            var equippedItem = context.State.EquippedItem;
            if (equippedItem == null || !ItemIsLetter(equippedItem.Item))
            {
                yield break;
            }
            yield return new ActorAction(eActionKey.USE, $"Deposit {equippedItem.Item.DisplayName}", gameObject);
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            var equippedItem = actor.State.EquippedItem;
            if (equippedItem != null &&
                action.Key == eActionKey.USE && action.State == eActionState.End)
            {
                actor.State.DropItem(equippedItem.Item);
                equippedItem.Item.gameObject.SafeDestroy();
            }
        }
    }
}