using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public abstract class DebugGraph : MonoBehaviour
{
	public int HistorySeconds = 100;
	UILineRenderer LineRenderer => GetComponentInChildren<UILineRenderer>();
	Text Label => GetComponentInChildren<Text>();
	protected abstract float Value { get; }
	protected abstract string LabelText { get; }

	private struct Frame
	{
		public float Value;
		public float Time;
	}

	private Queue<Frame> m_frame = new Queue<Frame>();

	private void Update()
	{
		Label.text = LabelText;
		var t = Time.timeSinceLevelLoad;
		m_frame.Enqueue(new Frame { Value = Value, Time = t });
		while(m_frame.Peek().Time < t - HistorySeconds)
		{
			m_frame.Dequeue();
		}
		LineRenderer.Points =
			m_frame.Select(f => new Vector2(((f.Time - t) / (float)HistorySeconds) + 1, f.Value))
			.ToArray();
		LineRenderer.SetAllDirty();
	}
}
