using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : Interactable
{
	public string PlainText;
    protected string AlienText => LanguageUtility.Translate(PlainText);

	public override IEnumerable<string> GetActions()
	{
		yield return $"\"{PlainText}\"";
	}
}
