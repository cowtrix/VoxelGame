using Generation.Spawning;
using Interaction;
using NodeCanvas.DialogueTrees;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Voxul.Utilities;

namespace Actors
{
	[RequireComponent(typeof(DialogueTreeController))]
	public class NPCActor : Actor, IDialogueActor
	{
		public AutoProperty<NPCInteractable> Interactable;
		public DialogueTreeController Controller => GetComponent<DialogueTreeController>();
		public NavMeshAgent NavmeshAgent => GetComponent<NavMeshAgent>();
		public bool IsTalking => Controller.isRunning;

		protected override void Awake()
        {
            Interactable = new AutoProperty<NPCInteractable>(gameObject, (go) => go.GetComponentInChildren<NPCInteractable>());
			base.Awake();
        }

        private void Start()
        {
			NavmeshAgent.enabled = true;
		}

        public bool CanTalkTo(Actor context)
		{
			if (Interactable.Value.Actor || Controller.graph == null)
			{
				return false;
			}
			if(!LookAdapter.CanSee(context.transform.position + Vector3.up))
            {
				return false;
            }
			return !Controller.isRunning;
		}

		public Vector3 GetClosestBin()
        {
			return ObjectBin.Instances
				.Where(b => b.BinType == ObjectBin.eBinType.VEHICLE)
				.OrderBy(b => Vector3.Distance(b.transform.position, transform.position))
				.First()
				.transform.position;
		}

		public void InteractWithActor(Actor instigator)
		{
			Controller.SetActorReference(DialogueTree.SELF_NAME, this);
			Controller.StartDialogue(instigator);
		}
	}
}