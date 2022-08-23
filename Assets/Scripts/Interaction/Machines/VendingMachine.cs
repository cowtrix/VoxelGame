using Actors;
using Common;
using Interaction.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class VendingMachine : MonoBehaviour
    {
        [Serializable]
        public class VendingItem
        {
            public SimpleInteractable Button;
            public Item Prefab;
            public int Cost;
        }

        public List<VendingItem> Items;

        private void Start()
        {
            foreach (var item in Items)
            {
                item.Button.Name = item.Prefab.DisplayName;
                item.Button.Actions = new List<ActorAction>()
                {
                    new ActorAction(eActionKey.USE, $"Buy ({item.Cost}¢)", item.Button.gameObject)
                };
            }
        }

        public void ReceiveEvent(Actor context, ActorAction action)
        {
            foreach (var item in Items)
            {
                if (action.Source != item.Button.gameObject)
                {
                    continue;
                }
                if (!context.State.TryAdd(eStateKey.Credits, -item.Cost, $"Purchased {item.Prefab.DisplayName}"))
                {
                    return;
                }
                context.State.PickupItem(Instantiate(item.Prefab.gameObject).GetComponentByInterface<Item>(), true);
                return;
            }
        }
    }
}