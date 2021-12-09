public class NPCInteract : Interactable
{
	public NPCActor Actor;

	public override string DisplayName => Actor?.Name;

	public override void Use(Actor actor, string action)
	{
		Actor.InteractWithActor(actor, action);
		base.Use(actor, action);
	}
}
