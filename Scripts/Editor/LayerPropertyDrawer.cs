#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    [CustomPropertyDrawer(typeof(Layer))]
    public class AvatarLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty layerIndex = property.FindPropertyRelative("layerIndex");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            layerIndex.intValue = EditorGUI.LayerField(position, layerIndex.intValue);
            EditorGUI.EndProperty();
        }
    }
}
#endif
