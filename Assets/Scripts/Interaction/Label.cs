using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : Interactable
{
	public string PlainText;

	public override string DisplayName => PlainText;

	protected string AlienText => LanguageUtility.Translate(PlainText);

	public override IEnumerable<string> GetActions(Actor context)
	{
		yield break;			   
	}
}
