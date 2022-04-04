using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Voxul;

namespace Interaction.Activities
{
	public class ConveyorBelt : ExtendedMonoBehaviour
	{
		public enum eNodeType
		{
			Transit, Smelter, Forge,
		}

		[FormerlySerializedAs("In")]
		public List<Vector3> Paths = new List<Vector3>();

		public List<ConveyorBelt> Neighbours;

		public Vector3 Center;
		public eNodeType Type;
		public float MoveSpeed = 1;
		public float HoldTime = 0;

		public ForgeResource CurrentItem { get; set; }

		[ContextMenu("Balance Neighbours")]
		public void BalanceNeighbours()
		{
			foreach (var n in Neighbours)
			{
				if (!n.Neighbours.Contains(this))
				{
					n.Neighbours.Add(this);
				}
			}
			Voxul.Utilities.Util.TrySetDirty(this);
		}

		protected virtual void OnDrawGizmosSelected()
		{
			foreach (var n in Neighbours)
			{
				Gizmos.DrawLine(transform.position, n.transform.position);
			}

			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Center, Vector3.one * .1f);
			Gizmos.color = Color.green;
			foreach (var inVec in Paths)
			{
				Gizmos.DrawLine(Center, inVec);
			}
		}

		public bool Receive(ForgeResource rsc, ConveyorBelt next)
		{
			if (CurrentItem)
			{
				return false;
			}
			StartCoroutine(ReceiveItem(rsc, next));
			return true;
		}

		protected virtual IEnumerator ReceiveItem(ForgeResource rsc, ConveyorBelt next)
		{
			CurrentItem = rsc;
			var wPos = transform.localToWorldMatrix.MultiplyPoint3x4(Center);
			var t = rsc.transform;
			while (t && t.position.y != wPos.y)
			{
				t.position = Vector3.MoveTowards(t.position, wPos, MoveSpeed * Time.deltaTime);
				yield return null;
			}
			while (t && t.position.xz() != wPos.xz())
			{
				t.position = Vector3.MoveTowards(t.position, wPos.xz().x0z(t.position.y), MoveSpeed * Time.deltaTime);
				yield return null;
			}
			if (!t)
			{
				yield break;
			}
			var holdTime = HoldTime;
			while (holdTime > 0)
			{
				holdTime -= Time.deltaTime;
				OnHold(1 - Mathf.Clamp01(holdTime / HoldTime));
				yield return null;
			}
			if (CurrentItem.CurrentLocation)
			{
				CurrentItem.CurrentLocation.CurrentItem = null;
			}
			CurrentItem.CurrentLocation = this;
			CurrentItem.TransitionLocation = null;
		}

		protected virtual void OnHold(float normalizedHoldTime) { }

		public Vector3 GetClosestNode(Vector3 wPos)
		{
			var bestDistance = float.MaxValue;
			Vector3 bestNode = default;
			foreach (var path in Paths)
			{
				var wPath = transform.localToWorldMatrix.MultiplyPoint3x4(path);
				var d = Vector3.Distance(wPos, wPath);
				if (d < bestDistance)
				{
					bestNode = wPath;
				}
			}
			return bestNode;
		}
	}
}