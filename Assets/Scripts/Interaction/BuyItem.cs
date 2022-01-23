using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyItem : Label
{
    public uint Cost = 10;

	public override IEnumerable<string> GetActions(Actor actor)
	{
		if (!CanUse(actor))
		{
			yield break;
		}
		yield return $"Buy {this.PlainText} [{Cost}c]";
	}
}
