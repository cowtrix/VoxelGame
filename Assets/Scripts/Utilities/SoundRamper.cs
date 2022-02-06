using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction.Utilities.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class SoundRamper : ExtendedMonoBehaviour
	{
		public float TransitionSpeed = 1f;
		public float TargetVolume = 1;
		public float DownPitch, UpPitch;

		public float Amount { get; set; }
		private float m_targetAmount;

		public AudioSource Source => GetComponent<AudioSource>();

		public void RampUp()
		{
			m_targetAmount = 1;
		}

		public void RampDown()
		{
			m_targetAmount = 0;
		}

		private void Update()
		{
			Amount = Mathf.MoveTowards(Amount, m_targetAmount, TransitionSpeed * Time.deltaTime);
			Source.volume = Mathf.Lerp(0, TargetVolume, Amount);
			Source.pitch = Mathf.Lerp(DownPitch, UpPitch, Amount);
		}
	}
}