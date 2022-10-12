using System;
using UnityEngine;

namespace Actors
{
	public enum eActionState
	{
		Start,
		Tick,
		End
	}

	[Serializable]
	public class ActorAction
	{
		public eActionKey Key;
		public string Description;
		public eActionState State;
		[HideInInspector]
		public Vector2 VectorContext;
		[HideInInspector]
		public GameObject Source;

		public ActorAction(eActionKey key, string description, GameObject source)
        {
			Key = key;
			Description = description;
			State = default;
			VectorContext = default;
			Source = source;
        }

		public ActorAction(eActionKey key, eActionState state, GameObject source, Vector2 vectorContext = default)
        {
			Key = key;
			Description = default;
			State = state;
			VectorContext = vectorContext;
			Source = source;
		}

		public override bool Equals(object obj)
		{
			return obj is ActorAction action &&
				   Key == action.Key;
		}

		public override int GetHashCode()
		{
			return 990326508 + Key.GetHashCode();
		}

		public static bool operator ==(ActorAction left, ActorAction right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ActorAction left, ActorAction right)
		{
			return !(left == right);
		}

		public override string ToString() => $"{Description} [{CameraController.Instance.Input.GetControlNameForAction(Key)}]";
	}
}