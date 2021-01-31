using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinItem : InteractableItem
{
	AudioSource Source => GetComponent<AudioSource>();
	public ParticleSystem Particles;
	public void Destr()
	{
		Source.Play();
		Particles.Play();
	}
	public override string Verb => InputManager.Instance.HeldItem ? "Destroy" : "";
}
