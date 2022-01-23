using Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Interaction.Activities
{
	public class Instrument : ExtendedMonoBehaviour
	{
		public InstrumentCollection Collection;
		public List<InstrumentKey> Keys { get; private set; }

		private void Awake()
		{
			Keys = new List<InstrumentKey>(GetComponentsInChildren<InstrumentKey>()
				.OrderBy(x => transform.worldToLocalMatrix.MultiplyPoint3x4(x.transform.position).z));
			for (int i = 0; i < Keys.Count; i++)
			{
				var key = Keys[i];
				var index = i;
				key.InteractionSettings.OnUsed.AddListener(a => OnNotePlayed(a, index));
			}
		}

		private void OnNotePlayed(Actor a, int i)
		{
			Debug.Log($"Attempting to play note {i}");
			var source = ObjectPool<AudioSource>.Get();
			source.spatialBlend = 1;
			source.transform.position = transform.position;
			source.PlayOneShot(Collection.Notes[i]);
			ObjectPool<AudioSource>.Release(source);
		}
	}
}