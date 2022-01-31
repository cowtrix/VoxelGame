using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Voxul;

namespace Interaction
{
	[Serializable]
	public class ActorEvent : UnityEvent<Actor>
	{
	}

	[Serializable]
	public class ActorActionEvent : UnityEvent<Actor, ActorAction>
	{
	}

	public interface IInteractable
	{
		void ExecuteAction(Actor actor, ActorAction action);
		IEnumerable<ActorAction> GetActions(Actor context);
		Transform transform { get; }
		GameObject gameObject { get; }
	}

	[Serializable]
	public class SpriteEvent : UnityEvent<Sprite> { }

	public abstract class Interactable : ExtendedMonoBehaviour, IInteractable
	{
		[Serializable]
		public class InteractableSettings
		{
			public ActorEvent OnFocusEnter;
			public ActorEvent OnFocusExit;
			public ActorActionEvent OnUsed;
			public ActorEvent OnEnterAttention;
			public ActorEvent OnExitAttention;

			public Func<Sprite> Icon;

			public float MaxFocusDistance = 5;
			public float MaxUseDistance = 2;
		}

		public InteractableSettings InteractionSettings = new InteractableSettings();

		public virtual IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (!CanUse(context))
			{
				yield break;
			}
			yield return new ActorAction { Key = eActionKey.USE, Description = "Use" };
		}

		protected virtual bool CanUse(Actor context)
		{
			var distance = Vector3.Distance(context.transform.position, transform.position);
			return distance <= InteractionSettings.MaxUseDistance;
		}
		public List<Collider> Colliders;

		public Bounds Bounds
		{
			get
			{
				if (Colliders == null || Colliders.Count == 0)
				{
					return default;
				}
				var bounds = Colliders[0].bounds;
				for (int i = 1; i < Colliders.Count; i++)
				{
					bounds.Encapsulate(Colliders[i].bounds);
				}
				return bounds;
			}
		}

		public abstract string DisplayName { get; }

		public Mesh GetInteractionMesh()
		{
			var voxelRenderer = GetComponent<VoxelRenderer>();
			if (voxelRenderer)
			{
				return voxelRenderer.Submeshes.FirstOrDefault()?.MeshFilter?.sharedMesh;
			}
			var thisFilter = GetComponent<MeshFilter>() ?? GetComponentInChildren<MeshFilter>();
			if (thisFilter)
			{
				return thisFilter.sharedMesh;
			}
			return null;
		}

		public virtual void ExitFocus(Actor actor)
		{
			InteractionSettings.OnFocusExit.Invoke(actor);
		}

		public virtual void EnterFocus(Actor actor)
		{
			InteractionSettings.OnFocusEnter.Invoke(actor);
		}

		public virtual void ExecuteAction(Actor actor, ActorAction action)
		{
			InteractionSettings.OnUsed.Invoke(actor, action);
		}

		public virtual void ExitAttention(Actor actor)
		{
			InteractionSettings.OnExitAttention.Invoke(actor);
		}

		public virtual void EnterAttention(Actor actor)
		{
			InteractionSettings.OnEnterAttention.Invoke(actor);
		}
	}
}