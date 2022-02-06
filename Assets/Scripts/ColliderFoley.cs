using Generation;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderFoley : MonoBehaviour
{
	[Range(0, 1)]
	public float Volume = .5f;
	public AudioClip[] ImpactClips;

	private void OnCollisionEnter(Collision collision)
	{
		var tempAudioSource = ObjectPool<AudioSource>.Get();
		tempAudioSource.transform.position = transform.position;
		tempAudioSource.spatialBlend = 1;
		tempAudioSource.PlayOneShot(ImpactClips.Random(), Volume);
		ObjectPool<AudioSource>.Release(tempAudioSource);
	}
}
