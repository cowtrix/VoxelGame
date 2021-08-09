using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Voxul;

[Serializable]
public class ActorEvent : UnityEvent<Actor> { }

[Serializable]
public class SpriteEvent : UnityEvent<Sprite> { }

public abstract class Interactable : ExtendedMonoBehaviour
{
	[Serializable]
	public class InteractableSettings
	{
		public ActorEvent OnFocusEnter;
		public ActorEvent OnFocusExit;
		public ActorEvent OnUsed;
		public ActorEvent OnEnterAttention;
		public ActorEvent OnExitAttention;

		public Func<Sprite> Icon;

		public float MaxFocusDistance = 5;
		public float MaxUseDistance = 2;

		public string[] Verbs;
	}

	public InteractableSettings InteractionSettings = new InteractableSettings();

	public virtual IEnumerable<string> GetActions()
	{
		yield return "Use";
	}

	public Collider[] Colliders => GetComponentsInChildren<Collider>();
	public Bounds Bounds
	{
		get
		{
			if(Colliders.Length == 0)
			{
				return default;
			}
			var bounds = Colliders[0].bounds;
			for (int i = 1; i < Colliders.Length; i++)
			{
				bounds.Encapsulate(Colliders[i].bounds);
			}
			return bounds;
		}
	}

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
	}

	public virtual void EnterFocus(Actor actor)
	{
	}

	public virtual void Use(Actor actor, string action)
	{
	}

	public virtual void ExitAttention(Actor actor)
	{
	}

	public virtual void EnterAttention(Actor actor)
	{
	}

}
