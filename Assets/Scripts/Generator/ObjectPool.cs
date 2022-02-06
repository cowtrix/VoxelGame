using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
	public static class ObjectPool<T> where T : class
	{
		private static Queue<T> m_pool = new Queue<T>();

		public static T Get()
		{
			if (m_pool.Count == 0)
			{
				if (typeof(Component).IsAssignableFrom(typeof(T)))
				{
					var newObj = new GameObject($"ObjectPool_{typeof(T)}");
					newObj.hideFlags = HideFlags.HideAndDontSave;
					m_pool.Enqueue(newObj.AddComponent(typeof(T)) as T);
				}
				else
				{
					throw new System.Exception($"Unsupported type {typeof(T)}");
				}
			}
			return m_pool.Dequeue();
		}

		public static void Release(T obj)
		{
			m_pool.Enqueue(obj);
		}
	}
}