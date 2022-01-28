using Actors;
using UnityEngine;

namespace Interaction.Activities
{
	public class InstrumentKey : Interactable
	{
		public Instrument Parent => GetComponentInParent<Instrument>();
		public string Note;
		public override string DisplayName => Note;

		public AudioClip Sound;

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			if (Sound)
			{
				var source = ObjectPool<AudioSource>.Get();
				source.spatialBlend = 1;
				source.transform.position = transform.position;
				source.PlayOneShot(Sound);
				ObjectPool<AudioSource>.Release(source);
			}
			base.ExecuteAction(actor, action);
		}
	}
}