using Actors;
using Interaction.Items;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities
{
    public class EquippableItemComponent : ItemComponent, IEquippableItemComponent
    {
        public Vector3 EquippedOffset, EquippedRotation;
        public Actor EquippedActor { get; private set; }
        public bool EquipOnPickup => m_equipOnPickup;
        [SerializeField]
        private bool m_equipOnPickup;

        public virtual void OnEquip(Actor actor)
        {
            EquippedActor = actor;
            transform.SetParent(actor.EquippedItemTransform);
            transform.localPosition = EquippedOffset;
            transform.localRotation = Quaternion.Euler(EquippedRotation);
            gameObject.SetActive(true);
            var rb = Item.Rigidbody;
            if (rb)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
                rb.position = actor.EquippedItemTransform.position;
            }
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }
        }

        public virtual void OnUnequip(Actor actor)
        {
            EquippedActor = null;
            transform.SetParent(actor.State.InventoryContainer);
            gameObject.SetActive(false);
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = true;
            }
        }

        public virtual void OnEquippedThink(Actor actor)
        {
            transform.localPosition = EquippedOffset;
            transform.localRotation = Quaternion.Euler(EquippedRotation);
        }

        public virtual void UseOn(Actor playerInteractionManager, GameObject target)
        {
        }

        public override bool ReceiveAction(Actor actor, ActorAction action)
        {
            if (action.Key == eActionKey.EQUIP && action.State == eActionState.End)
            {
                actor.State.EquipItem(this);
                return true;
            }
            if (action.Key == eActionKey.EXIT && action.State == eActionState.End)
            {
                actor.State.EquipItem(null);
                return true;
            }
            return false;
        }

        public override IEnumerable<ActorAction> GetActions(Actor actor)
        {
            var hasInventory = actor.GetComponentInChildren<Phone.PhoneController>();
            if (EquippedActor)
            {
                if (hasInventory)
                {
                    yield return new ActorAction(eActionKey.EXIT, "Put Away", gameObject);
                }
                yield break;
            }
            if (hasInventory)
            {
                yield return new ActorAction(eActionKey.USE, "Pick Up", gameObject);
                yield return new ActorAction(eActionKey.EQUIP, "Equip", gameObject);
            }
            else
            {
                yield return new ActorAction(eActionKey.USE, "Pick Up", gameObject);
            }
        }
    }
}