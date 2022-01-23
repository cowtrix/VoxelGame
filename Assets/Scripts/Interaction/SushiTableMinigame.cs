using Actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction.Activities.SushiTable
{
	public class SushiTableMinigame : Interactable
	{
		public bool Active { get; private set; }

		public override string DisplayName => "Sushi";

		public float MaxUseDistance = 6;

		private List<Interactable> m_subInteractables;

		private void Start()
		{
			m_subInteractables = new List<Interactable>(GetComponentsInChildren<Interactable>());
			foreach(var i in m_subInteractables)
			{
				i.enabled = false;
			}
		}

		public override void Use(Actor actor, string action)
		{
			Active = true;
			base.Use(actor, action);
			StartCoroutine(PlayGame(actor));
		}

		IEnumerator PlayGame(Actor actor)
		{
			while (Vector3.Distance(transform.position, actor.transform.position) < MaxUseDistance)
			{

				yield return null;
			}
			Active = false;
		}
	}
}
