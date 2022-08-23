using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

public class DialogueManager : Singleton<DialogueManager>
{
	public GameObject DialoguePrefab;

	public override void Awake() { Subscribe(); base.Awake(); }
	void OnEnable() { UnSubscribe(); Subscribe(); }
	void OnDisable() { UnSubscribe(); }

	Dictionary<IDialogueActor, DialogueUGUI> m_activeDialogues = new Dictionary<IDialogueActor, DialogueUGUI>();

	void Subscribe()
	{
		DialogueTree.OnDialogueStarted += OnDialogueStarted;
		DialogueTree.OnDialoguePaused += OnDialoguePaused;
		DialogueTree.OnDialogueFinished += OnDialogueFinished;
		DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
		DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
	}

	private DialogueUGUI GetOrCreateDialogue(IDialogueActor actor)
	{
		if(m_activeDialogues.TryGetValue(actor, out var dialogue) && dialogue)
		{
			return dialogue;
		}
		var newPrefab = Instantiate(DialoguePrefab).GetComponentInChildren<DialogueUGUI>();
		m_activeDialogues[actor] = newPrefab;
		return newPrefab;
	}

	private DialogueUGUI GetDialogue(IDialogueActor actor)
	{
		if (m_activeDialogues.TryGetValue(actor, out var dialogue) && dialogue)
		{
			return dialogue;
		}
		return null;
	}

	private void OnMultipleChoiceRequest(DialogueTree dlg, MultipleChoiceRequestInfo info)
	{
		var dialogue = GetOrCreateDialogue(dlg.Self);
		dialogue.OnMultipleChoiceRequest(info);
	}

	private void OnSubtitlesRequest(DialogueTree dlg, SubtitlesRequestInfo info)
	{
		var dialogue = GetOrCreateDialogue(dlg.Self);
		dialogue.OnSubtitlesRequest(info);
	}

	private void OnDialogueFinished(DialogueTree dlg)
	{
		CameraController.Instance.UIEnabled = false;
		var dialogue = GetOrCreateDialogue(dlg.Self);
		dialogue.OnDialogueFinished(dlg);
	}

	private void OnDialoguePaused(DialogueTree dlg)
	{
		var dialogue = GetOrCreateDialogue(dlg.Self);
		dialogue.OnDialoguePaused(dlg);
	}

	private void OnDialogueStarted(DialogueTree dlg)
	{
		CameraController.Instance.UIEnabled = true;
		var dialogue = GetOrCreateDialogue(dlg.Self);
		dialogue.OnDialogueStarted(dlg);
	}

	void UnSubscribe()
	{
		DialogueTree.OnDialogueStarted -= OnDialogueStarted;
		DialogueTree.OnDialoguePaused -= OnDialoguePaused;
		DialogueTree.OnDialogueFinished -= OnDialogueFinished;
		DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
		DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
	}

	

}
