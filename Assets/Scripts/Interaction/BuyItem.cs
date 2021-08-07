using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyItem : Label
{
    public uint Cost = 10;

	public override string GetText()
	{
		return $"Buy {this.PlainText} [{Cost}c]";
	}
}
