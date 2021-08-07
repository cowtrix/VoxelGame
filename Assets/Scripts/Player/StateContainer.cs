using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Voxul;

public class StateContainer : ExtendedMonoBehaviour
{
	private Dictionary<string, FieldInfo> m_fieldLookup;

	private void Awake()
	{
		m_fieldLookup = GetType()
			.GetFields()
			.Where(p => !p.DeclaringType.Assembly.FullName.Contains("UnityEngine.CoreModule"))
			.ToDictionary(f => f.Name, f => f);
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
		fieldInfoA.SetValue(this, a + b);
		return true;
	}
}