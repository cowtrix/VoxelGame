using Interaction;
using NodeCanvas.DialogueTrees;
using UnityEngine;
using UnityEngine.AI;
using Voxul.Utilities;

namespace Actors
{
	[RequireComponent(typeof(DialogueTreeController))]
	public class NPCActor : Actor, IDialogueActor
	{
		public AutoProperty<NPCInteractable> Interactable;
		public DialogueTreeController Controller => GetComponent<DialogueTreeController>();

        protected override void Awake()
        {
            Interactable = new AutoProperty<NPCInteractable>(gameObject, (go) => go.GetComponentInChildren<NPCInteractable>());
			base.Awake();
        }

        public bool CanTalkTo(Actor context)
		{
			if (Interactable.Value.Actor || Controller.graph == null)
			{
				return false;
			}
			return !Controller.isRunning;
		}

		public bool IsTalking()
        {
			return Controller.isRunning;
        }

		public void InteractWithActor(Actor instigator)
		{
			Controller.SetActorReference(DialogueTree.SELF_NAME, this);
			Controller.StartDialogue(instigator);
		}

		public void StopInteractingWithActor(Actor actor)
		{
			if (!Controller.isRunning)
			{
				return;
			}
			Controller.StopDialogue();
		}
	}
}