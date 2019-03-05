using UnityEditor;
using UnityEngine;

namespace QuickFind.Settings {
    [System.Serializable]
    internal struct WindowProperties {
        [SerializeField]
        private float m_width;
        internal float Width => m_width;
        [SerializeField]
        private int m_rows;
        internal int ExpandedRows => m_rows;

        internal WindowProperties (float w, int r) {
            m_width = w;
            m_rows = r;
        }
    }

    [CustomPropertyDrawer (typeof (WindowProperties))]
    public class WindowPropertyDrawer : PropertyDrawer {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField (position, "Window", EditorStyles.boldLabel);
            position.y += EditorGUIUtility.singleLineHeight + GUI.skin.label.margin.bottom;
            EditorGUI.PropertyField (position, property.FindPropertyRelative ("m_width"));
            position.y += EditorGUIUtility.singleLineHeight + GUI.skin.label.margin.bottom;
            EditorGUI.PropertyField (position, property.FindPropertyRelative ("m_rows"));
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return 3 * (EditorGUIUtility.singleLineHeight + GUI.skin.label.margin.bottom) - GUI.skin.label.margin.bottom;
        }
    }
}