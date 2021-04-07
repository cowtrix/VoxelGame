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
	public double OneBarTime => (60.0 / BPM) * 8.0;
	public double FourBarTime => OneBarTime * 4.0;
	public IEnumerable<SyncedAudioComponent> AudioSources => SyncedAudioComponent.Instances;

	public int BPM = 150;
	public double BPS => 1 / (BPM / 60f);
	public double BPMFrac => (AudioSettings.dspTime % BPS) / BPS;
	public double BPMSawtooth => Math.Abs((BPMFrac - .5f)) * 2;
	public double BPMFourBar => (AudioSettings.dspTime % (BPS * 8)) / (BPS * 8);
	public double BPMOneBar => (AudioSettings.dspTime % (BPS * 4)) / (BPS * 4);
	public double SecondsUntilNextBar => OneBarTime - (BPMOneBar * OneBarTime);
}
