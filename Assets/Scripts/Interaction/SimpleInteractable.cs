using Actors;
using System.Collections.Generic;

namespace Interaction
{
	public class SimpleInteractable : Interactable
	{
		public override string DisplayName => Name;

		public string Name;
		public string[] Actions = new string[1];

		public override IEnumerable<string> GetActions(Actor context)
		{
			if (!CanUse(context))
			{
				yield break;
			}
			foreach (var a in Actions)
			{
				yield return a;
			}
		}

		private void Update()
		{
		}
	}
}