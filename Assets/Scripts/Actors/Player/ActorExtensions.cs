using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace Actors
{
	public static class ActorExtensions
	{
		static eActionKey[] m_keys = Enum.GetValues(typeof(eActionKey)).Cast<eActionKey>().ToArray();
		public static eActionKey GetActionKey(this InputAction.CallbackContext cntxt)
		{
			foreach (var k in m_keys)
			{
				if (string.Equals(k.ToString(), cntxt.action.name, StringComparison.OrdinalIgnoreCase))
				{
					return k;
				}
			}
			throw new Exception($"Input action with no key: {cntxt.action.name}");
		}

		public static string GetControlNameForAction(this PlayerInput input, eActionKey key)
		{
			foreach (var action in input.actions)
			{
				if (string.Equals(action.name, key.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return action.GetBindingDisplayString();
				}
			}
			throw new Exception($"Couldn't find a control name for {key}");
		}
	}
}