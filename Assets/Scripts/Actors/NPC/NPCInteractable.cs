public class NPCInteractable : Interactable
{
	public NPCActor Self;
	public override string DisplayName => Self.DisplayName;

	public override void Use(Actor instigator, string action)
	{
		Self.InteractWithActor(instigator, action);
		base.Use(instigator, action);
	}
}
