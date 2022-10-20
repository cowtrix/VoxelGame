using Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Interaction
{
    public class TransformMover : ExtendedMonoBehaviour
    {
        [Serializable]
        public class TransformMove
        {
            public Transform Transform;
            public Vector3 OpenPosition, OpenRotation;
            public Vector3 ClosedPosition, ClosedRotation;
        }
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public bool RotationEnabled = true;
        public bool PositionEnabled = true;

        [Range(0, 1)]
        public float OpenAmount;
        private float m_lastOpenAmount;

        public List<TransformMove> Transforms;
        public float Speed = 1;
        private float m_targetOpen;

        private void OnValidate()
        {
            m_targetOpen = OpenAmount;
        }

        protected void Start()
        {
            m_targetOpen = OpenAmount;
        }

        private void Update()
        {
            OpenAmount = Mathf.MoveTowards(OpenAmount, m_targetOpen, Speed * Time.deltaTime);
            if (OpenAmount == m_lastOpenAmount)
            {
                return;
            }
            m_lastOpenAmount = OpenAmount;
            var smoothedLerp = Curve.Evaluate(OpenAmount);
            ThinkTransforms(smoothedLerp);
        }

        private void ThinkTransforms(float time)
        {
            foreach (var t in Transforms.Where(t => t.Transform))
            {
                if (PositionEnabled)
                {
                    t.Transform.localPosition = Vector3.Lerp(t.ClosedPosition, t.OpenPosition, time);
                }
                if (RotationEnabled)
                {
                    t.Transform.localRotation = Quaternion.Euler(Vector3.Lerp(t.ClosedRotation, t.OpenRotation, time));
                }
                t.Transform.TrySetDirty();
            }
        }

        [ContextMenu("Open")]
        public void OpenInstant()
        {
            m_targetOpen = 1;
            OpenAmount = 1;
            ThinkTransforms(m_targetOpen);
        }
        [ContextMenu("Close")]
        public void CloseInstant()
        {
            m_targetOpen = 0;
            OpenAmount = 0;
            ThinkTransforms(m_targetOpen);
        }

        public void Open()
        {
            m_targetOpen = 1;
        }

        public void Close()
        {
            m_targetOpen = 0;
        }
    }
}