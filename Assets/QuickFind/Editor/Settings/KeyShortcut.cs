using System.Text;
using UnityEditor;
using UnityEngine;

namespace QuickFind.Settings {
    [System.Serializable]
    internal struct KeyShortcut {
        [SerializeField]
        private EventModifiers m_modifiers;
        public EventModifiers Modifiers => m_modifiers;
        [SerializeField]
        private KeyCode m_key;
        public KeyCode Key => m_key;

        [SerializeField]
        private string m_command;
        [SerializeField]
        private string m_description;

        public KeyShortcut (string c, string d, EventModifiers m, KeyCode k) {
            m_command = c;
            m_description = d;
            m_modifiers = m;
            m_key = k;
        }
    }

    [CustomPropertyDrawer (typeof (KeyShortcut))]
    public class KeyShortcutPropertyDrawer : PropertyDrawer {
        private bool keySelect;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            var keyProp = property.FindPropertyRelative ("m_key");
            var modProp = property.FindPropertyRelative ("m_modifiers");

            var key = (KeyCode) keyProp.intValue;
            var modKeys = (EventModifiers) modProp.intValue;
            StringBuilder openKeyCombo = null;
            if (key != 0) {
                openKeyCombo = new StringBuilder (key.ToString ());
            }

            bool alt = modKeys.HasFlag (EventModifiers.Alt);
            if (alt) {
                openKeyCombo?.Insert (0, "Alt + ");
            }
            bool shift = modKeys.HasFlag (EventModifiers.Shift);
            if (shift) {
                openKeyCombo?.Insert (0, "Shift + ");
            }
#if UNITY_EDITOR_OSX
            bool control = modKeys.HasFlag (EventModifiers.Command);
            if (control) {
                openKeyCombo?.Insert (0, "Command + ");
            }
#else
            bool control = modKeys.HasFlag (EventModifiers.Control);
            if (control) {
                openKeyCombo?.Insert (0, "Ctrl + ");
            }
#endif

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.LabelField ("Key:", EditorStyles.boldLabel, GUILayout.Width (100));
            if (keySelect && Event.current.isKey && Event.current.keyCode == KeyCode.Escape) {
                keySelect = false;
                Event.current.Use ();
            }
            if (GUILayout.Button (keySelect? "[Enter a key]": openKeyCombo == null? "None": openKeyCombo.ToString (), GUI.skin.textField)) {
                keySelect = true;
            }
            if (GUILayout.Button ("Clear")) {
                keySelect = false;
                keyProp.intValue = 0;
                modProp.intValue = 0;
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.LabelField ("Modifiers:", EditorStyles.boldLabel, GUILayout.Width (100));
            EditorGUILayout.BeginVertical ();
#if UNITY_EDITOR_OSX
            EditorGUI.BeginChangeCheck ();
            control = EditorGUILayout.Toggle ("Command", control);
            if (EditorGUI.EndChangeCheck ()) {
                modKeys ^= EventModifiers.Command;
            }
#else
            EditorGUI.BeginChangeCheck ();
            control = EditorGUILayout.Toggle ("Control", control);
            if (EditorGUI.EndChangeCheck ()) {
                modKeys ^= EventModifiers.Control;
            }
#endif
            EditorGUI.BeginChangeCheck ();
            shift = EditorGUILayout.Toggle ("Shift", shift);
            if (EditorGUI.EndChangeCheck ()) {
                modKeys ^= EventModifiers.Shift;
            }

            EditorGUI.BeginChangeCheck ();
            alt = EditorGUILayout.Toggle ("Alt", alt);
            if (EditorGUI.EndChangeCheck ()) {
                modKeys ^= EventModifiers.Alt;
            }

            modProp.intValue = (int) modKeys;
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();

            // Description
            EditorGUILayout.LabelField ("Description", EditorStyles.boldLabel);
            EditorGUILayout.LabelField (property.FindPropertyRelative ("m_description").stringValue);

            if (keySelect && Event.current.type == EventType.KeyDown &&
#if UNITY_EDITOR_OSX
                Event.current.keyCode != KeyCode.Command &&
#else
                Event.current.keyCode != KeyCode.LeftControl &&
                Event.current.keyCode != KeyCode.RightControl &&
#endif
                Event.current.keyCode != KeyCode.LeftAlt &&
                Event.current.keyCode != KeyCode.RightAlt) {
                keySelect = false;
                keyProp.intValue = (int) Event.current.keyCode;
                modProp.intValue = (int) Event.current.modifiers;
                if (Event.current.shift) {
                    modProp.intValue |= (int) EventModifiers.Shift;
                }
                Event.current.Use ();
            }
        }
    }
}