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

		[Range(0, 1)]
		public float OpenAmount;
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
				yield return new ActorAction { Key = eActionKey.USE, Description = "Close Door" };
			}
			else
			{
				yield return new ActorAction { Key = eActionKey.USE, Description = "Open Door" };
			}
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
			foreach(var t in Transforms.Where(t => t.Transform))
			{
				t.Transform.localPosition = Vector3.Lerp(t.ClosedPosition, t.OpenPosition, OpenAmount);
				t.Transform.localRotation = Quaternion.Euler(Vector3.Lerp(t.ClosedRotation, t.OpenRotation, OpenAmount));
			}			
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

		public void Open() => m_targetOpen = 1;

		public void Close() => m_targetOpen = 0;
	}
}