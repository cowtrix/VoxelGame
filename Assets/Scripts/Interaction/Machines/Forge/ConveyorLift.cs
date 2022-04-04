using System.Collections;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class ConveyorLift : ConveyorBelt
	{
		public Transform LiftFloor;

		protected override IEnumerator ReceiveItem(ForgeResource rsc, ConveyorBelt next)
		{
			CurrentItem = rsc;
			var nextCenter = Center.xz().x0z(transform.worldToLocalMatrix.MultiplyPoint3x4(next.transform.localToWorldMatrix.MultiplyPoint3x4(next.Center)).y);
			var wPos = transform.localToWorldMatrix.MultiplyPoint3x4(nextCenter);
			DebugHelper.DrawPoint(wPos, .1f, Color.yellow, 5);

			while (LiftFloor.position.y != rsc.transform.position.y - 0.5f)
			{
				LiftFloor.position = Vector3.MoveTowards(LiftFloor.position, LiftFloor.position.xz().x0z(rsc.transform.position.y - .5f), MoveSpeed * Time.deltaTime);
				yield return null;
			}

			var t = rsc.transform;
			while (t && t.position.xz() != wPos.xz())
			{
				t.position = Vector3.MoveTowards(t.position, wPos.xz().x0z(t.position.y), MoveSpeed * Time.deltaTime);
				yield return null;
			}
			while (t && t.position.y != wPos.y)
			{
				t.position = Vector3.MoveTowards(t.position, wPos, MoveSpeed * Time.deltaTime);
				LiftFloor.position = t.position - Vector3.up * .5f;
				yield return null;
			}
			if (!t)
			{
				yield break;
			}
			if (CurrentItem.CurrentLocation)
			{
				CurrentItem.CurrentLocation.CurrentItem = null;
			}
			CurrentItem.CurrentLocation = this;
			CurrentItem.TransitionLocation = null;
		}
	}
}