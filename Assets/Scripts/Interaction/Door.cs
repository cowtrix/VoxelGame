using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction
{
    [ExecuteAlways]
    public class Door : Interactable
    {
        [Serializable]
        public class DoorTransform
        {
            public Transform Transform;
            public Vector3 OpenPosition, OpenRotation;
            public Vector3 ClosedPosition, ClosedRotation;
        }
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public bool RotationEnabled = true;
        public bool PositionEnabled = true;
        public bool ExternallyDriven = false;
        [Range(0, 1)]
        public float OpenAmount;
        private float m_lastOpenAmount;

        public List<DoorTransform> Transforms;
        public string DoorName = "Door";
        public float Speed = 1;
        public bool Usable;
        private float m_targetOpen;

        public override string DisplayName => DoorName;

        public override bool CanUse(Actor context) => Usable && base.CanUse(context);

        public override IEnumerable<ActorAction> GetActions(Actor actor)
        {
            if (!CanUse(actor))
            {
                yield break;
            }
            if (m_targetOpen > 0)
            {
                yield return new ActorAction(eActionKey.USE, "Close Door", gameObject);
            }
            else
            {
                yield return new ActorAction(eActionKey.USE, "Open Door", gameObject);
            }
        }

        private void OnValidate()
        {
            m_targetOpen = OpenAmount;
        }

        protected override void Start()
        {
            m_targetOpen = OpenAmount;
            base.Start();
        }

        private void Update()
        {
            if (!ExternallyDriven)
            {
                OpenAmount = Mathf.MoveTowards(OpenAmount, m_targetOpen, Speed * Time.deltaTime);
            }
            if (OpenAmount == m_lastOpenAmount)
            {
                return;
            }
            m_lastOpenAmount = OpenAmount;
            var smoothedLerp = Curve.Evaluate(OpenAmount);
            foreach (var t in Transforms.Where(t => t.Transform))
            {
                if (PositionEnabled)
                {
                    t.Transform.localPosition = Vector3.Lerp(t.ClosedPosition, t.OpenPosition, smoothedLerp);
                }
                if (RotationEnabled)
                {
                    t.Transform.localRotation = Quaternion.Euler(Vector3.Lerp(t.ClosedRotation, t.OpenRotation, smoothedLerp));
                }
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if (action.Key != eActionKey.USE)
            {
                base.ReceiveAction(actor, action);
                return;
            }
            if (m_targetOpen <= 0)
            {
                m_targetOpen = 1;
            }
            else
            {
                m_targetOpen = 0;
            }
        }

        [ContextMenu("Open")]
        public void Open() => m_targetOpen = 1;
        [ContextMenu("Close")]
        public void Close() => m_targetOpen = 0;
    }
}