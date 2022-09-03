using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Generation
{
    [CustomPropertyDrawer(typeof(SeedAttribute))]
    public class SeedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            const int rerollWidth = 64;
            var valueRect = new Rect(position.x, position.y, position.width - rerollWidth, position.height);
            property.intValue = EditorGUI.IntField(valueRect, label, property.intValue);
            var buttonRect = new Rect(position.x + valueRect.width, position.y, rerollWidth, position.height);
            if(GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_PreMatCube")))
            {
                property.intValue = Random.Range(int.MinValue, int.MaxValue);
            }
            EditorGUI.EndProperty();
        }
    }
}