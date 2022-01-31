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
		public DialogueTreeController Controller => GetComponent<DialogueTreeController>();

		public bool CanTalkTo(Actor context)
		{
			return !Controller.isRunning;
		}

		public void InteractWithActor(Actor instigator, ActorAction action)
		{
			Controller.SetActorReference(DialogueTree.SELF_NAME, this);
			Controller.StartDialogue(instigator);
		}
	}
}