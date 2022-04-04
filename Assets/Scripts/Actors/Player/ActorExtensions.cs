using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace Actors
{
	public static class ActorExtensions
	{
		static eActionKey[] m_keys = Enum.GetValues(typeof(eActionKey)).Cast<eActionKey>().ToArray();
		static Dictionary<eActionKey, string> m_controlNameCache = new Dictionary<eActionKey, string>();

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
			//if(m_controlNameCache.TryGetValue(key, out var bindingName))
			{
				//return bindingName;
			}
			foreach (var action in input.actions)
			{
				if (string.Equals(action.name, key.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					var bindingName = action.GetBindingDisplayString(group: input.currentControlScheme ?? input.defaultControlScheme, options: InputBinding.DisplayStringOptions.DontOmitDevice);
					m_controlNameCache[key] = bindingName;
					return bindingName;
				}
			}
			throw new Exception($"Couldn't find a control name for {key}");
		}
	}
}