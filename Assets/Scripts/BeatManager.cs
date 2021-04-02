using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FloatEvent : UnityEvent<float> { }

public class BeatManager : Singleton<BeatManager>
{
	public float SyncTime = 12.8f;
	public List<AudioSource> AudioSources;

	public int BPM = 150;

	public float BPMFrac;
	public float BPMSawtooth;

	private float m_startTime;
	private float m_nextSync;

	private void Update()
	{
		m_startTime += Time.deltaTime;
		var bps = 1 / (BPM / 60f);
		BPMFrac = (m_startTime % bps) / bps;
		BPMSawtooth = Mathf.Abs((BPMFrac - .5f)) * 2;

		// sync
		m_nextSync -= Time.deltaTime;

		while (m_nextSync < 0)
		{
			m_nextSync += SyncTime;
			Debug.Log("Synced!");
			var first = AudioSources.First();
			foreach (var au in AudioSources.Skip(1))
			{
				au.time = first.time;
			}
		}
	}
}
