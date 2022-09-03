using Generation;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;

namespace Generation
{
    public class DynamicGeneratorMeshAttachmentNode : DynamicGeneratorNode
    {
        public VoxelRenderer Renderer => GetComponent<VoxelRenderer>();

        [Seed]
        public int Seed;
        public Vector2 Scale = new Vector2(1, 1);
        public Vector3 BaseScale = new Vector3(1, 1, 1);
        public ObjectSet Collection;

        protected override void GenerateInternal()
        {
            if (!Collection || !Collection.Data.Any() | !Renderer)
            {
                return;
            }
            var meshAttachment = Collection.GetWeightedRandom<MeshAttachmentConfiguration>();
            Renderer.Mesh = meshAttachment.Mesh;
            Renderer.Invalidate(false, false);
        }
    }
}