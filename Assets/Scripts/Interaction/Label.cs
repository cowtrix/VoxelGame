using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : Interactable
{
	public string PlainText;
    protected string AlienText => LanguageUtility.Translate(PlainText);

	public virtual string GetText() => PlainText;

	public override void EnterFocus(Actor actor)
	{
		HUDManager.Instance.ActionLabel.text = PlainText;
		base.EnterFocus(actor);
	}

	public override void ExitFocus(Actor actor)
	{
		HUDManager.Instance.ActionLabel.text = "";
		base.ExitFocus(actor);
	}
}
