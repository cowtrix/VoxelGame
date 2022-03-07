using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Common
{
	public abstract class TrackedObject<T> : ExtendedMonoBehaviour where T : TrackedObject<T>
	{
		public virtual bool TrackDisabled => false;
		private static Dictionary<string, T> m_instances = new Dictionary<string, T>();

		protected virtual void OnEnable()
		{
			m_instances.Add(GUID, this as T);
		}

		protected virtual void OnDisable()
		{
			if (TrackDisabled)
			{
				return;
			}
			m_instances.Remove(GUID);
		}

		protected virtual void OnDestroy()
		{
			m_instances.Remove(GUID);
		}

		public static IEnumerable<T> Instances
		{
			get
			{
				if (!Application.isPlaying)
				{
					foreach(var i in FindObjectsOfType<T>())
					{
						yield return i;
					}
				}
				else
				{
					foreach (var instance in m_instances)
					{
						yield return instance.Value;
					}
				}
			}
		}

		public static bool TryGetValue(string guid, out T val)
		{
			if (!m_instances.TryGetValue(guid, out val))
			{
				return false;
			}
			return true;
		}

		[HideInInspector]
		[SerializeField]
		private string m_guid;

		public string GUID
		{
			get
			{
				while (string.IsNullOrEmpty(m_guid) || m_instances.ContainsKey(m_guid))
				{
					m_guid = Guid.NewGuid().ToString();
					this.TrySetDirty();
				}
				return m_guid;
			}
		}
	}
}