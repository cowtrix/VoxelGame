using Actors;
using System.Collections.Generic;

namespace Interaction
{
    public class SimpleInteractable : Interactable
    {
        public override string DisplayName => Name;

        public string Name;
        public List<ActorAction> Actions = new List<ActorAction>();

        public override IEnumerable<ActorAction> GetActions(Actor context)
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
    }
}