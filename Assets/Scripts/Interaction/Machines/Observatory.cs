using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Activities
{
    public class Observatory : Activity
    {
        public override string DisplayName => "Observatory";
        public float RotateSpeed = 1;
        public float TiltSpeed = 1;

        public Transform TelescopeBase;
        public Transform Telescope;
        public Door TelescopeTilt;

        public Transform VizBase;
        public Transform VizTelescope;
        public float VizOffset = 90;

        private Vector2 m_move;

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (!CanUse(context))
                yield break;
            if (Actor == context)
            {
                yield return new ActorAction(eActionKey.MOVE, "Move Telescope", gameObject);
                yield return new ActorAction(eActionKey.EXIT, "Stop Using", gameObject);
                yield break;
            }
            else
            {
                yield return new ActorAction(eActionKey.USE, "Start Using", gameObject);
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            switch (action.Key)
            {
                case eActionKey.MOVE:
                    var delta = action.VectorContext;
                    Move(actor, delta);
                    return;
            }
            base.ReceiveAction(actor, action);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            TelescopeBase.Rotate(Vector3.up * RotateSpeed * m_move.x * dt);
            TelescopeTilt.OpenAmount = Mathf.Clamp01(TelescopeTilt.OpenAmount + TiltSpeed * -m_move.y * dt);

            VizBase.localRotation = Quaternion.Euler(0, VizOffset, 0) * TelescopeBase.localRotation;
            VizTelescope.localRotation = Telescope.localRotation;
        }

        private void Move(Actor actor, Vector2 delta)
        {
            m_move = delta;
        }
        public override void OnStopActivity(Actor actor)
        {
            m_move = default;
            base.OnStopActivity(actor);
        }

    }
}