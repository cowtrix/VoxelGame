using NodeCanvas.DialogueTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTreeController))]
public class NPCActor : Actor, IDialogueActor
{
	public static string SelfID = "SELF";
	public DialogueTreeController Controller => GetComponent<DialogueTreeController>();

	public void InteractWithActor(Actor instigator, string action)
	{
		Controller.SetActorReference(SelfID, this);
		Controller.StartDialogue(instigator);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(GetDialogueOffset(), Vector3.one * .05f);
	}
}
