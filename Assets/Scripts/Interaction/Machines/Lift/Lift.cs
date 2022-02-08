using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class Lift : ExtendedMonoBehaviour
	{
		public Rigidbody Rigidbody => GetComponent<Rigidbody>();
		public LiftLine Line { get; private set; }
		public LiftStop TargetStop { get; private set; }
		public bool IsMoving { get; internal set; }

		public Door Door;

		public float Speed = 1;
		public Transform ButtonContainer;
		public Button ButtonPrefab;

		public void SetData(LiftLine line)
		{
			Line = line;
			for(var i = 0; i < Line.Stops.Count; ++i)
			{
				var stop = Line.Stops[i];
				var newButton = Instantiate(ButtonPrefab.gameObject).GetComponent<Button>();
				newButton.onClick.AddListener(() => Line.RequestStop(stop));
				newButton.transform.SetParent(ButtonContainer);
				newButton.transform.Reset();
				newButton.GetComponentInChildren<Text>().text = LanguageUtility.Translate(i.ToString()).SafeSubstring(0, 1);
			}
			ButtonPrefab.gameObject.SetActive(false);
		}

		private void Start()
		{
			StartCoroutine(Think());
		}

		private IEnumerator Think()
		{
			while (true)
			{
				while (TargetStop == null || Door.OpenAmount > 0)
				{
					IsMoving = false;
					yield return null;
				}

				IsMoving = true;
				Debug.Log($"Lift moving to {TargetStop}");
				while ((Rigidbody.position - TargetStop.WorldPosition).magnitude > .01f)
				{
					Rigidbody.position = Vector3.MoveTowards(Rigidbody.position, TargetStop.WorldPosition, Speed * Time.deltaTime);
					yield return null;
				}
				Debug.Log($"Lift arrived at {TargetStop}");
				Line.StoppedAt(TargetStop);
				Door.Open();
				TargetStop.Door.Open();
				yield return new WaitForSeconds(5);
				Door.Close();
				TargetStop.Door.Close();
				TargetStop = null;
			}
			
		}

		public void SetTarget(LiftStop stop)
		{
			TargetStop = stop;
			Debug.Log($"Set lift stop to {TargetStop}");
		}
	}
}