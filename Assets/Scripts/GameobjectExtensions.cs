using UnityEngine;

public static class GameobjectExtensions
{
	public static T GetOrAddComponent<T>(this GameObject go) where T: Component
	{
		var c = go?.GetComponent<T>();
		if(!c)
		{
			c = go?.AddComponent<T>();
		}
		return c;
	}
}
