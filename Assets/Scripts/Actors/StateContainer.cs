using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Voxul;
using Common;
using Interaction.Items;

namespace Actors
{
	[Serializable]
	public class FloatStateUpdateEvent : UnityEvent<Actor, string, float, float> { }

	[Serializable]
	public class FloatStateFailureUpdateEvent : UnityEvent<Actor, string, float> { }

	[Serializable]
	public class InventoryStateUpdateEvent : UnityEvent<Actor, ActorState.eInventoryAction, Item> { }

	public interface IStateProvider
	{
		string GUID { get; }
		string GetSaveData();
		void LoadSaveData(string data);
	}

	[RequireComponent(typeof(Actor))]
	public abstract class StateContainer : TrackedObject<StateContainer>
	{
		private Dictionary<string, PropertyInfo> m_fieldLookup;
		public Actor Actor => GetComponent<Actor>();
		public FloatStateUpdateEvent OnStateUpdate = new FloatStateUpdateEvent();
		public FloatStateFailureUpdateEvent OnStateFailedUpdate = new FloatStateFailureUpdateEvent();

		public IStateProvider[] StateProviders { get; private set; }

		protected virtual void Awake()
		{
			StateProviders = gameObject.GetComponentsByInterfaceInChildren<IStateProvider>(true);

			m_fieldLookup = GetType()
				.GetProperties()
				.Where(p => p.CanWrite && p.CanRead && p.GetCustomAttribute<NonSerializedAttribute>() == null && !p.DeclaringType.Assembly.FullName.Contains("UnityEngine.CoreModule"))
				.ToDictionary(f => f.Name, f => f);

			foreach (var field in m_fieldLookup
				.Where(f => f.Value.PropertyType == typeof(float) || f.Value.PropertyType == typeof(int)))
			{
				OnStateUpdate.Invoke(Actor, field.Key, (float)Convert.ChangeType(field.Value.GetValue(this), typeof(float)), 0);
			}
		}

		public bool TryGetValue<T>(string name, out T result)
		{
			if (!m_fieldLookup.TryGetValue(name, out var fieldInfo))
			{
				result = default;
				return false;
			}
			var rawObject = fieldInfo.GetValue(this);
			result = (T)Convert.ChangeType(rawObject, typeof(T));
			return true;
		}

		public bool TryAdd(string fieldA, string fieldB)
		{
			if (!m_fieldLookup.TryGetValue(fieldA, out var fieldInfoA) || !m_fieldLookup.TryGetValue(fieldB, out var fieldInfoB))
			{
				return false;
			}

			dynamic a = fieldInfoA.GetValue(this);
			dynamic b = fieldInfoB.GetValue(this);
			var newVal = a + b;
			fieldInfoA.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, fieldA, newVal, b);
			//Debug.Log($"State update: {fieldA}: {newVal} ({b})");
			return true;
		}

		public bool TryAdd(string field, float delta)
		{
			if (!m_fieldLookup.TryGetValue(field, out var fieldInfo))
			{
				Debug.LogWarning($"Tried to delta {field} but it did not exist on actor {Actor}", this);
				return false;
			}
			/*if(fieldInfo.PropertyType != typeof(float))
			{
				throw new Exception($"State delta type mismatch for {field}, expected float but got {fieldInfo.PropertyType}");
			}*/
			var val = (float)fieldInfo.GetValue(this);
			var newVal = val + delta;

			var minAttr = fieldInfo.GetCustomAttribute<StateMinAttribute>();
			if (minAttr != null && newVal < minAttr.Min)
			{
				OnStateFailedUpdate.Invoke(Actor, field, delta);
				return false;
			}

			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, field, newVal, delta);
			//Debug.Log($"State update: {field}: {newVal} ({delta})");
			return true;
		}

		public bool TryAdd(string field, int delta)
		{
			if (!m_fieldLookup.TryGetValue(field, out var fieldInfo))
			{
				Debug.LogWarning($"Tried to delta {field} but it did not exist on actor {Actor}", this);
				return false;
			}
			/*if (fieldInfo.PropertyType != typeof(int))
			{
				throw new Exception($"State delta type mismatch for {field}, expected int but got {fieldInfo.PropertyType}");
			}*/
			var val = (int)fieldInfo.GetValue(this);
			var newVal = val + delta;

			var minAttr = fieldInfo.GetCustomAttribute<StateMinAttribute>();
			if (minAttr != null && newVal < minAttr.Min)
			{
				return false;
			}

			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, field, newVal, delta);
			//Debug.Log($"State update: {field}: {newVal} ({delta})");
			return true;
		}

		public JObject GetSaveData()
		{
			var data = new JObject();
			data[nameof(GUID)] = GUID;
			foreach (var f in m_fieldLookup)
			{
				data[f.Key] = JsonUtility.ToJson(f.Value.GetValue(this));
			}
			foreach (var provider in StateProviders)
			{
				var providerSaveData = provider.GetSaveData();
				data[provider.GUID] = providerSaveData;
				Debug.Log($"Provider {provider} loaded save data {providerSaveData}");
			}
			Debug.Log($"State container {this} saved data {data}");
			return data;
		}

		public void LoadSaveData(JObject data)
		{
			Debug.Log($"State container {this} loaded save data {data}");
			foreach (var token in data.Children())
			{
				if (!(token is JObject jObj))
				{
					Debug.LogError($"Unexpected token in save data: {token}", this);
					return;
				}

				var target = jObj.Path;
				if (m_fieldLookup.TryGetValue(target, out var prop))
				{
					var obj = JsonUtility.FromJson(jObj.ToString(), prop.PropertyType);
					prop.SetValue(this, obj);
					return;
				}

				var provider = StateProviders.FirstOrDefault(s => s.GUID == target);
				if (provider != null)
				{
					Debug.Log($"Provider {provider} loaded save data {jObj}");
					provider.LoadSaveData(jObj.ToString());
				}

				Debug.LogError($"Failed to find property with name: {target}", this);
			}
			OnSaveDataLoaded();
		}

		protected virtual void OnSaveDataLoaded() { }
	}
}