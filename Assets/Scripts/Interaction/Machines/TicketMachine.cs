using Actors;
using Interaction.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction
{
    public class TicketMachine : Interactable
    {
        public override string DisplayName => "Ticket Machine";
        public SplineSegment Spline;
        public EquippableItemBase Ticket;
        public int LineResolution = 10;
        public float PrintSpeed = 1;

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            yield return new ActorAction(eActionKey.USE, "Take a ticket", gameObject);
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if (action.Key == eActionKey.USE)
            {
                StartCoroutine(PrintTicket());
                return;
            }
            base.ReceiveAction(actor, action);
        }

        IEnumerator PrintTicket()
        {
            var ticketAmount = 0f;
            var newTicket = Instantiate(Ticket.gameObject).GetComponent<EquippableItemBase>();
            newTicket.transform.position = transform.localToWorldMatrix.MultiplyPoint3x4(Spline.Bounds.center);
            var line = newTicket.GetComponent<LineRenderer>();
            var rb = newTicket.GetComponent<Rigidbody>();
            rb.detectCollisions = false;
            while (ticketAmount < 1)
            {
                ticketAmount += Time.deltaTime * PrintSpeed;
                ticketAmount = Mathf.MoveTowards(ticketAmount, 1, Time.deltaTime * PrintSpeed);
                line.positionCount = LineResolution;
                for (var i = 0; i < LineResolution; ++i)
                {
                    var t = ticketAmount - (i / (float)(LineResolution - 1)) * ticketAmount;
                    var p = transform.localToWorldMatrix.MultiplyPoint3x4(Spline.GetUniformPointOnSpline(t));
                    p = line.transform.worldToLocalMatrix.MultiplyPoint3x4(p);
                    line.SetPosition(i, p);
                }
                yield return null;
            }
            rb.isKinematic = false;
            rb.GetComponent<GravityRigidbody>().enabled = true;
            rb.detectCollisions = true;
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Spline.Recalculate();
            Gizmos.matrix = transform.localToWorldMatrix;
            var lastP = Spline.Points[0].Position;
            foreach (var p in Spline.Points.Skip(1))
            {
                var nextP = p.Position;
                Gizmos.DrawLine(lastP, nextP);
                lastP = nextP;
            }
        }
    }
}