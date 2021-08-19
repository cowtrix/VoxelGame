using UnityEngine;

public class InstrumentKey : Interactable
{
	public Instrument Parent => GetComponentInParent<Instrument>();
	public string Note;
	public override string DisplayName => Note;

	public AudioClip Sound;

	public override void Use(Actor actor, string action)
	{
		if (Sound)
		{
			var source = ObjectPool<AudioSource>.Get();
			source.spatialBlend = 1;
			source.transform.position = transform.position;
			source.PlayOneShot(Sound);
			ObjectPool<AudioSource>.Release(source);
		}
		base.Use(actor, action);
	}
}