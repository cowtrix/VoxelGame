using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Voxul;
using Voxul.Edit;

namespace Generation
{
    [CustomEditor(typeof(MeshAttachment))]
    public class MeshAttachmentGUI : ExtendedMonobehaviourEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(targets.Length == 1 && target is MeshAttachment attachment && attachment.Configuration)
            {
                var editor = NativeEditorUtility.GetWrapper<MeshAttachmentConfiguration>();
                EditorGUILayout.BeginVertical("Box");
                editor.DrawGUI(attachment.Configuration);
                EditorGUILayout.EndVertical();
            }
        }
    }
}