using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction.Activities
{

	public class ForgeConveyorManager : SlowUpdater
	{
		public List<ConveyorBelt> Belts { get; set; }
		protected ConveyorBelt Smelter => Belts.Single(b => b.Type == ConveyorBelt.eNodeType.Smelter);
		protected IEnumerable<ConveyorBelt> Forges => Belts.Where(b => b.Type == ConveyorBelt.eNodeType.Forge);
		public int StackCount { get; set; } = 15;
		public ForgeResource ResourcePrefab;

		protected override int TickOnThread(float dt)
		{
			var smelter = Smelter;
			if (StackCount <= 0 || smelter.CurrentItem)
			{
				return 0;
			}
			StackCount--;
			var newItem = Instantiate(ResourcePrefab.gameObject).GetComponent<ForgeResource>();
			newItem.transform.position = smelter.transform.localToWorldMatrix.MultiplyPoint3x4(smelter.Center);
			newItem.Path = GetPath(smelter, Forges.Where(f => !f.CurrentItem).Random());
			newItem.gameObject.SetActive(true);
			smelter.CurrentItem = newItem;
			newItem.CurrentLocation = smelter;
			return 1;
		}

		private void Awake()
		{
			ResourcePrefab.gameObject.SetActive(false);
			Belts = new List<ConveyorBelt>(GetComponentsInChildren<ConveyorBelt>());
		}

		public List<ConveyorBelt> GetPath(ConveyorBelt origin, ConveyorBelt target)
		{
			var path = new Queue<ConveyorBelt>();
			var open = new Queue<ConveyorBelt>();
			open.Enqueue(origin);
			path.Enqueue(target);
			var explored = new HashSet<ConveyorBelt>();

			bool exploreNode(ConveyorBelt node, ConveyorBelt target, Queue<ConveyorBelt> path)
			{
				explored.Add(node);
				foreach (var n in node.Neighbours)
				{
					if (explored.Contains(n))
					{
						continue;
					}
					open.Enqueue(n);
					if (n == target || exploreNode(n, target, path))
					{
						path.Enqueue(n);
						return true;
					}
				}
				return false;
			}
			if (exploreNode(origin, target, path))
			{
				return path.ToList();
			}
			throw new System.Exception($"Cannot find a path between {origin} & {target}");
		}
	}
}