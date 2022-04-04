using UnityEngine;


namespace NodeCanvas.DialogueTrees
{
    ///<summary> An interface to use for DialogueActors within a DialogueTree.</summary>
	public interface IDialogueActor
    {
        string DisplayName { get; }
        Transform GetDialogueContainer();
        Transform transform { get; }
		bool HasSaid(IStatement key);
		void RecordSaid(IStatement statement);
	}
}