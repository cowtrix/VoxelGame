using Interaction;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.DialogueTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	[RequireComponent(typeof(DialogueTreeController))]
	public class NPCActor : Actor, IDialogueActor
	{
		public NPCInteractable Interactable;
		public DialogueTreeController Controller => GetComponent<DialogueTreeController>();

		public bool CanTalkTo(Actor context)
		{
			if (Interactable.Actor)
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

		public void StopInteractingWithActor(Actor actor)
		{
			Controller.StopDialogue();
		}
	}
}