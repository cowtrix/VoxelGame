using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
    public class MeshAttachment : ExtendedMonoBehaviour
    {
        public VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
        public MeshAttachmentConfiguration Configuration;
        public MeshAttachment Parent;
        public string ParentAttachmentPoint;

        private void OnDrawGizmosSelected()
        {
            if (Configuration == null)
            {
                return;
            }
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach(var attachment in Configuration.AttachmentPoints)
            {
                Gizmos.DrawWireCube(attachment.Position, Vector3.one * .1f);
            }
        }

        public void SetupComponents()
        {
            if (!Renderer)
            {
                gameObject.AddComponent<VoxelRenderer>();
            }
            Renderer.hideFlags = HideFlags.HideInInspector;
        }

        public void OnValidate()
        {
            if (Parent && Parent.Configuration)
            {
                foreach(var attachment in Parent.Configuration.AttachmentPoints)
                {
                    if(attachment.Name == ParentAttachmentPoint)
                    {
                        transform.localPosition = attachment.Position;
                        transform.TrySetDirty();
                    }
                }
            }
            if (!Configuration)
            {
                return;
            }
            SetupComponents();
            Renderer.Mesh = Configuration.Mesh;
            Renderer.GenerateCollider = false;
            Renderer.Invalidate(false, false);
            foreach(var child in GetComponentsInChildren<MeshAttachment>())
            {
                if(child == this)
                {
                    continue;
                }
                child.OnValidate();
            }
        }
    }
}