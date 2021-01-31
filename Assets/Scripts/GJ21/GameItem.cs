using System.Collections.Generic;

public class GameItem : InteractableItem 
{
	public List<eItemTrait> Traits;

	public override string Verb => InputManager.Instance.HeldItem ? "" : "Pick Up";
}

public enum eItemTrait
{
	Green,
	Red,
	Yellow,
	Blue,

	Tasty,
	Flammable,	// Destroys Wet
	Wet,		// Destroys Electric
	Electric,	// Destroys Flammable
}