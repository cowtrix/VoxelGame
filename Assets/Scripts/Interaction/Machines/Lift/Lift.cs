using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        public UnityEvent OnStartMoving, OnStopMoving;
        public Door Door;
        public float DoorPauseTime = 5;
        public float Speed = 1;

        public void SetData(LiftLine line)
        {
            Line = line;
            /*if (ButtonPrefab)
            {
                for (var i = 0; i < Line.Stops.Count; ++i)
                {
                    var stop = Line.Stops[i];
                    var newButton = Instantiate(ButtonPrefab.gameObject).GetComponent<SimpleInteractable>();
                    newButton.InteractionSettings.OnUsed.AddListener((a, c) => Line.RequestStop(stop));
                    newButton.transform.SetParent(transform);
                    newButton.transform.Reset();
                    newButton.GetComponentInChildren<TextMeshPro>().text = LanguageUtility.GetStringForTextMesh(i.ToString());
                }
                ButtonPrefab.gameObject.SetActive(false);
            }*/
        }

        public void RequestByIndex(int index)
        {
            Line.RequestStop(Line.Stops[index]);
        }

        private void Start()
        {
            StartCoroutine(Think());
        }

        private IEnumerator Think()
        {
            while (true)
            {
                while (TargetStop == null || Door?.OpenAmount > 0)
                {
                    IsMoving = false;
                    yield return null;
                }

                IsMoving = true;
                OnStartMoving?.Invoke();
                Debug.Log($"Lift moving to {TargetStop}");
                while ((Rigidbody.position - TargetStop.WorldPosition).magnitude > .01f)
                {
                    Rigidbody.position = Vector3.MoveTowards(Rigidbody.position, TargetStop.WorldPosition, Speed * Time.deltaTime);
                    yield return null;
                }
                Debug.Log($"Lift arrived at {TargetStop}");
                IsMoving = false;
                OnStopMoving?.Invoke();
                Line?.StoppedAt(TargetStop);
                Door?.Open();
                TargetStop.Door?.Open();
                TargetStop.LiftArrived(this);
                yield return new WaitForSeconds(DoorPauseTime);
                Door?.Close();
                TargetStop.Door?.Close();
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