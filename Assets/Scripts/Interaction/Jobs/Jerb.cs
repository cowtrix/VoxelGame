using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jerbs
{
    public abstract class Jerb : TrackedObject<Jerb>
    {
        [Serializable]
        public class JerbInfo 
        {
            public Sprite Icon;
            public string Name;
            [TextArea]
            public string Description;
            public string Reward;
            public Bounds Bounds;
            public string Instructions;
        }
        
        public JerbInfo JobInfo;

        public Vector3 GetCurrentPosition() => transform.position;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            GizmoExtensions.DrawWireCube(JobInfo.Bounds.center, JobInfo.Bounds.extents, Quaternion.identity, Color.green);
        }

    }
}