using Actors;
using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Activities
{
    public class LiftToggleLever : Interactable
    {
        public float UpAngle = -45;
        public float DownAngle = -120;
        public float RotateSpeed = 1;
        public Lift Lift;
        public LiftStop Top, Bottom;

        enum eLiftState
        {
            Ascended,
            Ascending,
            Descending,
            Descended,
        }
        private eLiftState m_state;

        public override string DisplayName => "Lift";

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            switch (action.Key)
            {
                case eActionKey.USE:
                    if(m_state != eLiftState.Ascending && m_state != eLiftState.Ascended)
                    {
                        Lift.SetTarget(Top);
                        m_state = eLiftState.Ascending;
                    }
                    else
                    {
                        Lift.SetTarget(Bottom);
                        m_state = eLiftState.Descending;
                    }
                    break;
            }
        }

        private void Update()
        {
            var targetAngle = (m_state == eLiftState.Ascending || m_state == eLiftState.Ascended) ? UpAngle : DownAngle;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(targetAngle, 0, 0), Time.deltaTime * RotateSpeed);
        }
    }
}