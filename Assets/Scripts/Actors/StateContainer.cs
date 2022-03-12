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
	public class FloatStateUpdateEvent : UnityEvent<Actor, StateUpdate<float>> { }

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
				if(!Enum.TryParse(typeof(eStateKey), field.Key, out var enumObj) || !(enumObj is eStateKey key))
				{
					continue;
				}
				var currentValue = (float)Convert.ChangeType(field.Value.GetValue(this), typeof(float));
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(key, null, currentValue, 0, true));
			}
		}

		public bool TryGetValue<T>(eStateKey key, out T result)
		{
			if (!m_fieldLookup.TryGetValue(key.ToString(), out var fieldInfo))
			{
				result = default;
				return false;
			}
			var rawObject = fieldInfo.GetValue(this);
			result = (T)Convert.ChangeType(rawObject, typeof(T));
			return true;
		}

		public bool TryAdd(eStateKey fieldA, eStateKey fieldB, string description)
		{
			if (!m_fieldLookup.TryGetValue(fieldA.ToString(), out var fieldInfoA) || !m_fieldLookup.TryGetValue(fieldB.ToString(), out var fieldInfoB))
			{
				return false;
			}
			dynamic a = fieldInfoA.GetValue(this);
			dynamic b = fieldInfoB.GetValue(this);

			var newVal = a + b;
			var minAttr = fieldInfoA.GetCustomAttribute<StateMinAttribute>();
			if (minAttr != null && newVal < minAttr.Min)
			{
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(fieldA, description, 0, a, false));
				return false;
			}

			fieldInfoA.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, new StateUpdate<float>(fieldA, description, b, newVal, true));
			//Debug.Log($"State update: {fieldA}: {newVal} ({b})");
			return true;
		}

		public bool TryAdd(eStateKey field, float delta, string description)
		{
			if (!m_fieldLookup.TryGetValue(field.ToString(), out var fieldInfo))
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
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(field, description, delta, val, false));
				return false;
			}

			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, new StateUpdate<float>(field, description, newVal, delta, true));
			//Debug.Log($"State update: {field}: {newVal} ({delta})");
			return true;
		}

		public bool TryAdd(eStateKey key, int delta, string desc)
		{
			if (!m_fieldLookup.TryGetValue(key.ToString(), out var fieldInfo))
			{
				Debug.LogWarning($"Tried to delta {key} but it did not exist on actor {Actor}", this);
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
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(key, desc, newVal, delta, false));
				return false;
			}

			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, new StateUpdate<float>(key, desc, newVal, delta, true));
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

				if (Enum.TryParse(typeof(eStateKey), jObj.Path, out var enumObj) 
					&& enumObj is eStateKey key
					&& m_fieldLookup.TryGetValue(key.ToString(), out var prop))
				{
					var obj = JsonUtility.FromJson(jObj.ToString(), prop.PropertyType);
					prop.SetValue(this, obj);
					return;
				}

				var provider = StateProviders.FirstOrDefault(s => s.GUID == jObj.Path);
				if (provider != null)
				{
					Debug.Log($"Provider {provider} loaded save data {jObj}");
					provider.LoadSaveData(jObj.ToString());
				}

				Debug.LogError($"Failed to find property with name: {jObj.Path}", this);
			}
			OnSaveDataLoaded();
		}

		protected virtual void OnSaveDataLoaded() { }
	}
}