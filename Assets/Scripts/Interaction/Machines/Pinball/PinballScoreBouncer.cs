using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction.Activities
{
	public class PinballScoreBouncer : MonoBehaviour
	{
		public float FadeSpeed = 1;
		public float BounceStrength;
		public PinballMachine Machine;
		public VoxelColorTint Tint => GetComponent<VoxelColorTint>();
		private Color m_targetColor;

		public AudioSource Ding;
		public Vector2 PitchRandom = new Vector2(.9f, 1.1f);

		private void Start()
		{
			m_targetColor = Tint.Color;
		}

		private void Update()
		{
            if (!Machine.Actor)
            {
				return;
            }
			Tint.Color = Color.Lerp(Tint.Color, m_targetColor, FadeSpeed * Time.deltaTime);
			Tint.Invalidate();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if(collision.collider.name != "Ball")
			{
				return;
			}
			Ding.pitch = Random.Range(PitchRandom.x, PitchRandom.y);
			Ding.Play();
			collision.rigidbody.AddExplosionForce(BounceStrength, transform.position, 1);
			Machine.CurrentScore++;
			Tint.Color = m_targetColor * 2;
		}
	}
}