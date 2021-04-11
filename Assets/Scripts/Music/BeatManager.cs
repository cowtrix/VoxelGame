using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BeatManager))]
public class BeatManagerGUI : Editor
{
	public override void OnInspectorGUI()
	{
		var bm = target as BeatManager;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("OneBarTime", bm.OneBarTime.ToString());
		EditorGUILayout.LabelField("FourBarTime", bm.FourBarTime.ToString());
		EditorGUILayout.LabelField("SecondsUntilNextBar", bm.SecondsUntilNextBar.ToString());
		EditorGUILayout.EndVertical();
		base.OnInspectorGUI();
	}
}

#endif

public class BeatManager : Singleton<BeatManager>
{
	public double OneBarTime => 60.0 / BPM * 8.0;
	public double FourBarTime => OneBarTime * 4.0;
	public IEnumerable<SyncedAudioComponent> AudioSources => SyncedAudioComponent.Instances;

	public int BPM = 150;

	[Range(2, 6)]
	public int BeatsPerBar = 4;

	public double BPS => 1 / (BPM / 60f);
	public double BPMFrac(double offset = 0) => ((AudioSettings.dspTime % BPS / BPS) + offset) % 1.0;
	public double BPMSawtooth(double offset = 0) => Math.Abs(BPMFrac(offset) - .5f) * (BeatsPerBar / 2.0);
	public double BPMFourBar(double offset = 0) => (AudioSettings.dspTime + offset) % (BPS * BeatsPerBar * 2) / (BPS * BeatsPerBar * 2);
	public double BPMOneBar(double offset = 0) => (AudioSettings.dspTime + offset) % (BPS * BeatsPerBar) / (BPS * BeatsPerBar);
	public double SecondsUntilNextBar => OneBarTime - (BPMOneBar() * OneBarTime);
}
