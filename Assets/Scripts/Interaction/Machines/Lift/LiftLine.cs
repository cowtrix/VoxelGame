using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Interaction.Activities
{
	public class LiftLine : BasicMachine
	{
		public enum eLiftDirection
		{
			Idle, Down, Up,
		}

		public float DoorTime = 5;
		public List<LiftStop> Stops { get; private set; }
		public Lift Lift;
		public Door LiftDoor => Lift.GetComponentInChildren<Door>();
		public eLiftDirection CurrentDirection { get; private set; }
		private HashSet<LiftStop> m_requestedStops = new HashSet<LiftStop>();

		private void Start()
		{
			Stops = GetComponentsInChildren<LiftStop>().OrderByDescending(t => t.transform.position.y).ToList();
			Lift.SetData(this);
			foreach(var s in Stops)
			{
				s.SetData(this);
			}
		}

        protected override int Tick(float dt)
        {
			var cost = base.Tick(dt) + 1;
			if (!m_requestedStops.Any() || Lift.IsMoving)
			{
				return cost;
			}
			IEnumerable<LiftStop> validStops;
			switch(CurrentDirection)
			{
				default:
					validStops = m_requestedStops.OrderBy(s => Vector3.Distance(Lift.transform.position, s.WorldPosition));
					break;
				case eLiftDirection.Up:
					validStops = m_requestedStops
						.Where(s => s.transform.position.y < Lift.transform.position.y)
						.OrderBy(s => Vector3.Distance(Lift.transform.position, s.WorldPosition));
					break;
				case eLiftDirection.Down:
					validStops = m_requestedStops
						.Where(s => s.transform.position.y < Lift.transform.position.y)
						.OrderBy(s => Vector3.Distance(Lift.transform.position, s.WorldPosition));
					break;
			}
			var closestStop = validStops.FirstOrDefault();
			if(closestStop == null)
			{
				CurrentDirection = eLiftDirection.Idle;
				return cost;
			}
			Lift.SetTarget(closestStop);
			return cost;
		}

		public void StoppedAt(LiftStop targetStop)
		{
			m_requestedStops.Remove(targetStop);
		}

		public void RequestStop(LiftStop stop)
		{
			m_requestedStops.Add(stop);
		}
	}
}