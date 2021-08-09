using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Voxul;

[Serializable]
public class StateUpdateEvent : UnityEvent<Actor, string, float, float> { }

[RequireComponent(typeof(Actor))]
public class StateContainer : ExtendedMonoBehaviour
{
	private Dictionary<string, FieldInfo> m_fieldLookup;
	public Actor Actor => GetComponent<Actor>();
	public StateUpdateEvent OnStateUpdate = new StateUpdateEvent();

	private IEnumerator Start()
	{
		m_fieldLookup = GetType()
			.GetFields()
			.Where(p => !p.DeclaringType.Assembly.FullName.Contains("UnityEngine.CoreModule"))
			.ToDictionary(f => f.Name, f => f);
		yield return null;
		foreach(var field in m_fieldLookup
			.Where(f => f.Value.FieldType == typeof(float) || f.Value.FieldType == typeof(int)))
		{
			OnStateUpdate.Invoke(Actor, field.Key, (float)Convert.ChangeType(field.Value.GetValue(this), typeof(float)), 0);
			yield return new WaitForSeconds(.2f);
		}
	}

	public bool TryGetValue<T>(string name, out T result)
	{
		if(!m_fieldLookup.TryGetValue(name, out var fieldInfo))
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
		if(!m_fieldLookup.TryGetValue(fieldA, out var fieldInfoA) || !m_fieldLookup.TryGetValue(fieldB, out var fieldInfoB))
		{
			return false;
		}
		dynamic a = fieldInfoA.GetValue(this);
		dynamic b = fieldInfoB.GetValue(this);
		var newVal = a + b;
		fieldInfoA.SetValue(this, a + b);
		OnStateUpdate.Invoke(Actor, fieldA, newVal, b);
		return true;
	}

	public bool TryAdd(string field, float delta)
	{
		if (!m_fieldLookup.TryGetValue(field, out var fieldInfo))
		{
			return false;
		}
		var val = (float)fieldInfo.GetValue(this);
		var newVal = val + delta;
		fieldInfo.SetValue(this, newVal);
		OnStateUpdate.Invoke(Actor, field, newVal, delta);
		return true;
	}

	public bool TryAdd(string field, int delta)
	{
		if (!m_fieldLookup.TryGetValue(field, out var fieldInfo))
		{
			return false;
		}
		var val = (int)fieldInfo.GetValue(this);
		var newVal = val + delta;
		fieldInfo.SetValue(this, newVal);
		OnStateUpdate.Invoke(Actor, field, newVal, delta);
		return true;
	}
}