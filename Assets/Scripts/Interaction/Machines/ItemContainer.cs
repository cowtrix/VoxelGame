using Actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Items
{
    public class ItemContainer : Interactable
    {
        public override string DisplayName => Name;

        public string Name = "Untitled Container";

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            var equippedItem = context.State.EquippedItem;
            if (equippedItem == null)
            {
                yield break;
            }
            yield return new ActorAction(eActionKey.USE, $"Dispose of {equippedItem.Item.DisplayName}", gameObject);
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            var equippedItem = actor.State.EquippedItem;
            if (equippedItem != null && action.Key == eActionKey.USE && action.State == eActionState.End)
            {
                actor.State.DropItem(equippedItem.Item);
                equippedItem.OnPickup(null);
                equippedItem.Item.transform.SetParent(transform);
                equippedItem.Item.gameObject.SetActive(false);
            }
        }
    }
}