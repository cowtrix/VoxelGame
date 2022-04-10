using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Actors.NPC
{
	public class ActorVoice : ExtendedMonoBehaviour
	{
		public AudioSource Audio;
		public List<AudioClip> VoiceClips;

		public void SayWord(string w)
		{
			if (Audio.isPlaying)
			{
				return;
			}
			Random.InitState(w.GetHashCode());
			Audio.clip = VoiceClips.Random();
			Audio.Play();
		} 
	}
}