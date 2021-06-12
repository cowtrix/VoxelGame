using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public abstract class TrackedObject<T> : MonoBehaviour where T : TrackedObject<T>
	{
		public virtual bool TrackDisabled => false;
		private static HashSet<T> m_instances = new HashSet<T>();

		protected virtual void OnEnable()
		{
			m_instances.Add(this as T);
		}

		protected virtual void OnDisable()
		{
			if (TrackDisabled)
			{
				return;
			}
			m_instances.Remove(this as T);
		}

		protected virtual void OnDestroy()
		{
			m_instances.Remove(this as T);
		}

		public static IReadOnlyCollection<T> Instances => m_instances;
	}
}