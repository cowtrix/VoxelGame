using NodeCanvas.DialogueTrees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCActor : Actor, IDialogueActor
{
	public string Name = "Untitled NPC";
	public Texture2D portrait => null;
	public Sprite portraitSprite => null;
	public Color dialogueColor => Color.white;
	public Vector3 dialoguePosition => Vector3.zero;

	public void InteractWithActor(Actor actor, string action)
	{
		if(actor is PlayerActor player)
		{
			player.DialogueController.StartDialogue(this);
		}
	}
}
