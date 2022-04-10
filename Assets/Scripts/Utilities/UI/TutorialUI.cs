using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

public class TutorialUI : ExtendedMonoBehaviour
{
	[Serializable]
	public class TutorialStep
	{
		public CanvasGroup Group;
		public eActionKey Key;
		public bool RequireDelta;
	}
	public PlayerActor Player;
	public float FadeSpeed = 1;
	public float MinimumWaitTime = 3;
	public List<TutorialStep> Steps;
	public int CurrentStep { get; private set; } = 0;
	public bool RequireDelta { get; private set; }
	public eActionKey? CurrentControlListen { get; private set; }

	private void Start()
	{
		foreach (var s in Steps)
		{
			s.Group.alpha = 0;
		}
		if (Player.OnActionExecuted == null)
		{
			Player.OnActionExecuted = new ActorActionEvent();
		}
		Player.OnActionExecuted.AddListener(OnActionExecuted);
		StartCoroutine(Think());
	}

	private void OnActionExecuted(ActorAction action)
	{
		if (!CurrentControlListen.HasValue)
		{
			return;
		}
		if (action.Key == CurrentControlListen.Value)
		{
			if (RequireDelta && action.Context.magnitude <= 0)
			{
				return;
			}
			CurrentControlListen = null;
		}
	}

	IEnumerator Think()
	{
		while (CurrentStep < Steps.Count)
		{
			var currentStep = Steps[CurrentStep];
			while (currentStep.Group.alpha < 1)
			{
				currentStep.Group.alpha += Time.deltaTime * FadeSpeed;
				yield return null;
			}
			var minT = Time.time + MinimumWaitTime;
			CurrentControlListen = currentStep.Key;
			RequireDelta = currentStep.RequireDelta;
			while (minT > Time.time || CurrentControlListen.HasValue)
			{
				yield return null;
			}
			while (currentStep.Group.alpha > 0)
			{
				currentStep.Group.alpha -= Time.deltaTime * FadeSpeed;
				yield return null;
			}
			CurrentStep++;
			yield return null;
		}
	}
}
