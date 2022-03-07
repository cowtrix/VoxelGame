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
	public struct ActorAction
	{
		public eActionKey Key;
		public string Description;
		public eActionState State;
		public Vector2 Context;

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