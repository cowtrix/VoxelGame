using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzak : MonoBehaviour
{
	public AudioSource AudioSource;
    public AudioClip[] Clips;
    public float Pause = 60;

	private void Start()
	{
		StartCoroutine(PlayMusic());
	}

	IEnumerator PlayMusic()
	{
		var pause = new WaitForSeconds(Pause);
		while (true)
		{
			var c = Clips.Random();
			AudioSource.PlayOneShot(c);
			yield return new WaitForSeconds(c.length);
			yield return pause;
		}
	}
}
