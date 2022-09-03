using Generation;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Voxul;
using Voxul.Meshing;
using Voxul.Utilities;

namespace Generation
{
    public class DynamicGeneratorMeshAttachmentNode : DynamicGeneratorNode
    {
        public MeshAttachment Attachment => GetComponent<MeshAttachment>();
        public Vector2 Scale = new Vector2(1, 1);
        public Vector3 BaseScale = new Vector3(1, 1, 1);
        public ObjectSet Collection;

        protected override void GenerateInternal()
        {
            if (!Collection || !Collection.Data.Any() | !Attachment)
            {
                return;
            }
            Random.InitState(Seed);
            var meshAttachment = Collection.GetWeightedRandom<MeshAttachmentConfiguration>();
            Attachment.Configuration = meshAttachment;
            Attachment.Invalidate();
        }
    }
}