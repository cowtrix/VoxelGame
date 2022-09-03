using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Meshing;

namespace Generation
{
    [CreateAssetMenu(menuName = "Custom/Generation/Mesh Attachment Configuration")]
    public class MeshAttachmentConfiguration : ScriptableObject
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/VoxelMesh/Create Mesh Attachment")]
        public static void CreateNewAttachment(UnityEditor.MenuCommand command)
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(command.context);
            var newAttachment = CreateInstance<MeshAttachmentConfiguration>();
            newAttachment.Mesh = command.context as VoxelMesh;
            UnityEditor.AssetDatabase.CreateAsset(newAttachment, assetPath.Replace(".asset", "") + "_meshAttachment.asset");
        }
#endif

        [Serializable]
        public class AttachmentPoint
        {
            public string Name;
            public Vector3 Position;
        }

        public VoxelMesh Mesh;
        public List<AttachmentPoint> AttachmentPoints = new List<AttachmentPoint>();
    }
}