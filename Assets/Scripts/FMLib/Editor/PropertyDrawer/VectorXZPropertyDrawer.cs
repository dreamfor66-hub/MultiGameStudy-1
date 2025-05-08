using Sirenix.OdinInspector.Editor;
using FMLib.Structs;
using UnityEditor;
using UnityEngine;

namespace FMLib.Editor.PropertyDrawer
{
    public class VectorXZPropertyDrawer : OdinValueDrawer<VectorXZ>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            var left = new Rect(rect.x, rect.y, rect.width / 2f - 1, rect.height);
            var right = new Rect(rect.x + rect.width / 2f + 1, rect.y, rect.width / 2f - 1, rect.height);
            var vec = ValueEntry.SmartValue;
            vec.x = EditorGUI.FloatField(left, vec.x);
            vec.z = EditorGUI.FloatField(right, vec.z);
            ValueEntry.SmartValue = vec;
        }
    }
}
