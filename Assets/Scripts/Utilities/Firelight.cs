using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firelight : MonoBehaviour
{
	[Header("Light")]
	public Gradient Gradient;
	public Vector2 Intensity = new Vector2(1, 2);
	public float FlickerSpeed = 1;
	public int Seed;
    public Light Light;

	[Header("Audio")]
	public float Volume = .5f;
	public AudioSource Audio;
	public AudioClip StartSound;

	private float m_ramp;

	protected void Update()
	{
		m_ramp = Mathf.Clamp01(m_ramp + Time.deltaTime);
		Light.color = Gradient.Evaluate(Perlin.Noise(Time.time * FlickerSpeed + Seed));
		Light.intensity = m_ramp * Mathf.Lerp(Intensity.x, Intensity.y, Perlin.Noise(Time.time * FlickerSpeed - Seed));
        if (Audio)
        {
			Audio.volume = m_ramp * Volume;
		}
	}

	private void OnEnable()
	{
		if (Audio)
		{
			Audio.Play();
		}
		Light.enabled = true;
		Light.intensity = 0;
		if (Audio && StartSound)
		{
			Audio.volume = Volume;
			Audio.PlayOneShot(StartSound);
		}
		m_ramp = 0;
	}

	private void OnDisable()
	{
		if (Audio)
		{
			Audio.Stop();
		}
		Light.enabled = false;
	}
}
