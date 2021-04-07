using Common;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SyncedAudioComponent : TrackedObject<SyncedAudioComponent>
{
	[Range(1, 4)]
	public int BarLength;
	public AudioSource Source => GetComponent<AudioSource>();

	private void Start()
	{
		Source.Stop();
		Source.playOnAwake = false;
		Source.enabled = false;
		StartCoroutine(Synchronise());
	}

	AudioSource SetupTempSource(int i)
	{
		var src = Source;
		var go = new GameObject($"tempAudio_{i}");
		go.hideFlags = HideFlags.DontSave;
		go.transform.SetParent(transform);
		var audioSource = go.AddComponent<AudioSource>();
		audioSource.volume = src.volume;
		audioSource.spatialBlend = src.spatialBlend;
		audioSource.clip = src.clip;
		return audioSource;
	}

	public IEnumerator Synchronise()
	{
		var bm = BeatManager.Instance;
		var as1 = SetupTempSource(1);
		var as2 = SetupTempSource(2);
		var cs = as1;
		while (true)
		{
			// Swap sources
			if (cs.name == as1.name)
				cs = as2;
			else
				cs = as1;

			cs.clip = Source.clip;
			cs.Stop();
			var st = AudioSettings.dspTime + bm.SecondsUntilNextBar;
			cs.PlayScheduled(st);
			var et = st + bm.OneBarTime;
			cs.SetScheduledEndTime(et);

			while(AudioSettings.dspTime < st)
			{
				yield return null;
			}
		}
	}
}
