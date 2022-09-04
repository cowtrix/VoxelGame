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
		public bool IsTalking => Controller.isRunning;

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
			if(!LookAdapter.CanSee(context.transform.position + Vector3.up))
            {
				return false;
            }
			return !Controller.isRunning;
		}

		public void InteractWithActor(Actor instigator)
		{
			Controller.SetActorReference(DialogueTree.SELF_NAME, this);
			Controller.StartDialogue(instigator);
		}
	}
}