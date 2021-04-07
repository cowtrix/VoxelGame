using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SyncedAudioComponent))]
public class BarMixer : MonoBehaviour
{
	[Range(1, 4)]
	public int BarWait = 1;
	protected AudioSource Source => GetComponent<AudioSource>();
	protected SyncedAudioComponent Sync => GetComponent<SyncedAudioComponent>();
	public AudioClip[] Clips;

	private void Start()
	{
		StartCoroutine(RotateClips());
	}

	private IEnumerator RotateClips()
	{
		while(true)
		{
			Source.clip = Clips.Random();

			var wait = BeatManager.Instance.SecondsUntilNextBar / (5 - BarWait);
			if(wait < 1f)
			{
				wait += BeatManager.Instance.SecondsUntilNextBar / (5 - BarWait);
			}
			Debug.Log($"Rotated clip to {Source.clip}. Waiting for {wait}s");
			yield return new WaitForSeconds((float)wait);
		}
	}
}