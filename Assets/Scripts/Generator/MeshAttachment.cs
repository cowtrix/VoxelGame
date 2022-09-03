using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
    [DisallowMultipleComponent]
    public class MeshAttachment : ExtendedMonoBehaviour
    {
        public VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
        public MeshAttachmentConfiguration Configuration;
        public MeshAttachment Parent;
        public string ParentAttachmentPoint;

        private void OnDrawGizmosSelected()
        {
            if (!Configuration || Configuration == null)
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
            Renderer.hideFlags = HideFlags.None;
        }

        [ContextMenu("Invalidate")]
        public void Invalidate()
        {
            if (Parent && Parent.Configuration)
            {
                foreach (var attachment in Parent.Configuration.AttachmentPoints)
                {
                    if (attachment.Name == ParentAttachmentPoint)
                    {
                        transform.localPosition = attachment.Position;
                        transform.TrySetDirty();
                    }
                }
            }
            if (!Configuration)
            {
                if (Renderer)
                {
                    Renderer.Mesh = null;
                    Renderer.Invalidate(false, false, true);
                }
                return;
            }
            SetupComponents();
            Renderer.Mesh = Configuration.Mesh;
            Renderer.GenerateCollider = false;
            Renderer.Invalidate(false, false, true);
            foreach (var child in GetComponentsInChildren<MeshAttachment>())
            {
                if (child == this)
                {
                    continue;
                }
                child.Invalidate();
            }
            gameObject.TrySetDirty();
        }
    }
}