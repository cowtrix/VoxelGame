using Actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
	[ExecuteAlways]
	public class Door : Interactable
	{
		[Range(0, 1)]
		public float OpenAmount;
		public Vector3 OpenPosition, OpenRotation;
		public Vector3 ClosedPosition, ClosedRotation;
		public string DoorName = "Door";
		public float Speed = 1;
		private float m_targetOpen;

		public override string DisplayName => DoorName;

		public override IEnumerable<ActorAction> GetActions(Actor actor)
		{
			if (!CanUse(actor))
			{
				yield break;
			}
			if (m_targetOpen > 0)
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Close Door" };
			}
			else
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Open Door" };
			}
		}

		private void Reset()
		{
			OpenPosition = transform.localPosition;
			ClosedPosition = transform.localPosition;
		}

		private void OnValidate()
		{
			m_targetOpen = OpenAmount;
		}

		private void Start()
		{
			m_targetOpen = OpenAmount;
		}

		private void Update()
		{
			transform.localPosition = Vector3.Lerp(ClosedPosition, OpenPosition, OpenAmount);
			transform.localRotation = Quaternion.Lerp(Quaternion.Euler(ClosedRotation), Quaternion.Euler(OpenRotation), OpenAmount);

			OpenAmount = Mathf.MoveTowards(OpenAmount, m_targetOpen, Speed * Time.deltaTime);
		}

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			if(action.Key != eActionKey.USE)
			{
				base.ExecuteAction(actor, action);
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
	}
}