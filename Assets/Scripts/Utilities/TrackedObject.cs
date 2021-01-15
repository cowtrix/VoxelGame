using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public class TrackedObject<T> : MonoBehaviour where T: TrackedObject<T>
	{
		private static HashSet<T> m_instances = new HashSet<T>();

		protected virtual void OnEnable()
		{
			m_instances.Add(this as T);
		}

		protected virtual void OnDisable()
		{
			m_instances.Remove(this as T);
		}

		public static IReadOnlyCollection<T> Instances => m_instances;
	}
}