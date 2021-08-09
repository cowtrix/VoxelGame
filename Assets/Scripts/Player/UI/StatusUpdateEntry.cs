using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FloatEvent : UnityEvent<float> { }

[Serializable]
public class StringEvent : UnityEvent<string> { }

public class StatusUpdateEntry : MonoBehaviour
{
    public string StateName;
	public StatusUpdate Parent;
	public PlayerState PlayerState { get; private set; }
    public FloatEvent Delta, Value;
	public StringEvent DeltaString, ValueString;

	public float MoveSpeed = 1;
	public float ActiveTimeout = 5;
	private float m_lastUpdate;
	private bool m_active;
	private void Awake()
	{
		PlayerState = CameraController.Instance.GetComponentInParent<PlayerState>();
		PlayerState.OnStateUpdate.AddListener(StateUpdate);

		StartCoroutine(Think());
	}

	private void StateUpdate(Actor actor, string fieldName, float value, float delta)
	{
		if(fieldName != StateName)
		{
			return;
		}
		Delta.Invoke(delta);
		if(delta == 0)
		{
			DeltaString.Invoke("");
		}
		else
		{
			DeltaString.Invoke($"({delta})");
		}
		Value.Invoke(value);
		ValueString.Invoke(value.ToString());
		m_lastUpdate = Time.time;
		m_active = true;
	}

	private IEnumerator Think()
	{
		while (true)
		{
			if (m_active)
			{
				if (Time.time > m_lastUpdate + ActiveTimeout)
				{
					// timed out, deactivate
					m_active = false;
					continue;
				}
				var parentRectTransform = Parent.GetComponent<RectTransform>();
				if(transform.parent != parentRectTransform)
				{
					Vector3 targetPos;
					do
					{
						targetPos = new Vector3(0, parentRectTransform.position.y + parentRectTransform.sizeDelta.y)
							+ new Vector3(2, -4);
						yield return null;
						transform.position = Vector3.Lerp(transform.position, targetPos, MoveSpeed * Time.deltaTime);
					}
					while (Vector3.Distance(transform.position, targetPos) > 0.01f);
					transform.SetParent(Parent.transform);
				}
			}
			else
			{
				transform.SetParent(Parent.InactiveContainer);
				var targetPos = new Vector3(0, 0);
				transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, MoveSpeed * Time.deltaTime);
			}
			yield return null;
		}
		
	}
}
