using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Interaction.Activities
{
	public class LiftStop : MonoBehaviour
	{
		public LiftLine Line { get; private set; }
		public Toggle UpButton, DownButton;
		public Vector3 Offset;
		public Door Door;

		public Vector3 WorldPosition => transform.localToWorldMatrix.MultiplyPoint3x4(Offset);

		public int StopIndex => Line.Stops.IndexOf(this);

		public bool LiftIsAtStop => Vector3.Distance(Line.Lift.Rigidbody.position, WorldPosition) < .3f;

		public void SetData(LiftLine line)
		{
			Line = line;
			var stopIndex = StopIndex;
			if (stopIndex == 0)
			{
				UpButton?.gameObject.SetActive(false);
			}
			else if (stopIndex == line.Stops.Count - 1)
			{
				DownButton?.gameObject.SetActive(false);
			}
			DownButton?.onValueChanged.AddListener(b => { if (b) { OnClick(LiftLine.eLiftDirection.Down); } });
			UpButton?.onValueChanged.AddListener(b => { if (b) { OnClick(LiftLine.eLiftDirection.Up); } });
		}

		private void OnClick(LiftLine.eLiftDirection direction)
		{
			if (!LiftIsAtStop)
			{
				Line.RequestStop(this);
				return;
			}
			else
			{
				StartCoroutine(OpenDoors());
			}
		}

		private IEnumerator OpenDoors()
		{
			Door?.Open();
			Line.LiftDoor?.Open();

			yield return new WaitForSeconds(Line.DoorTime);

			Door?.Close();
			Line.LiftDoor?.Close();
		}

        private void OnDrawGizmosSelected()
        {
			Gizmos.DrawWireSphere(WorldPosition, .1f);
			Gizmos.matrix = transform.worldToLocalMatrix;
			Gizmos.DrawCube(Offset, Vector3.one * .1f);
        }
    }
}