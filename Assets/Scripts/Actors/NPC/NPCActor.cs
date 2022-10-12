using Generation.Spawning;
using Interaction;
using NodeCanvas.BehaviourTrees;
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
		public DialogueTreeController Dialogue => GetComponent<DialogueTreeController>();
		public BehaviourTreeOwner BehaviourTree => GetComponent<BehaviourTreeOwner>();
		public NavMeshAgent NavmeshAgent => GetComponent<NavMeshAgent>();
		public bool IsTalking => Dialogue.isRunning;

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
			if (Interactable.Value.Actor || Dialogue.graph == null)
			{
				return false;
			}
			if(!LookAdapter.CanSee(context.transform.position + Vector3.up))
            {
				return false;
            }
			return !Dialogue.isRunning;
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
			Dialogue.SetActorReference(DialogueTree.SELF_NAME, this);
			Dialogue.StartDialogue(instigator);
		}

		protected override int TickOffThread(float dt)
        {
			BehaviourTree.Tick();
			return base.TickOffThread(dt) + 1;
        }
	}
}