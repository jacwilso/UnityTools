using System.Text;
using UnityEditor;
using UnityEngine;

namespace QuickFind.Settings {
    public static class QuickFindPreferences {
        private static Vector2 shortcutScroll;
        private static SerializedProperty selectedShortcut;

        public class Styles {
            public readonly GUIStyle shortcutCommand = new GUIStyle (GUI.skin.label);

            public Styles () {
                shortcutCommand.onActive.background = GenerateTex (Color.blue);
            }

            private Texture2D GenerateTex (Color c) {
                var tex = new Texture2D (1, 1);
                for (int y = 0; y < 1; y++) {
                    for (int x = 0; x < 1; x++) {
                        tex.SetPixel (x, y, c);
                    }
                }
                tex.Apply ();
                return tex;
            }
        }
        private static Styles s_styles = null;
        public static Styles m_styles {
            get {
                if (s_styles == null) {
                    s_styles = new Styles ();
                }
                return s_styles;
            }
        }

#if !UNITY_2018_2_OR_NEWER
        [PreferenceItem ("Quick Find Settings")]
        public static void PreferencesGUI () {
            PreferncesGUI ();
        }
#endif

        internal static void PreferncesGUI () {
            var settings = QuickFindSettings.GetSerializedSettings ();

            // Key Shortcuts
            EditorGUILayout.BeginHorizontal ();
            shortcutScroll = EditorGUILayout.BeginScrollView (shortcutScroll, false, true, GUILayout.Width (180f));
            foreach (SerializedProperty shortcut in settings.FindProperty ("m_shortcuts")) {
                if (GUILayout.Button (shortcut.FindPropertyRelative ("m_command").stringValue, m_styles.shortcutCommand)) {
                    selectedShortcut = shortcut;
                }
                // EditorGUILayout.PropertyField (shortcut);
            }
            EditorGUILayout.EndScrollView ();
            if (selectedShortcut != null) {
                EditorGUILayout.BeginVertical ();
                EditorGUILayout.PropertyField (selectedShortcut);
                EditorGUILayout.EndVertical ();
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.LabelField ("Other Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField (settings.FindProperty ("m_typeFilterStartsWith"));
            EditorGUILayout.PropertyField (settings.FindProperty ("m_includeProjectFiles"));

            EditorGUILayout.PropertyField (settings.FindProperty ("m_window"));

            EditorGUILayout.PropertyField (settings.FindProperty ("m_fontSize"));

            settings.ApplyModifiedProperties ();

            EditorGUILayout.Space ();
            EditorGUILayout.LabelField ("Shortcuts", EditorStyles.boldLabel);
            EditorGUILayout.LabelField ("Ctrl + g: opens the window/expands window when its open");
            EditorGUILayout.LabelField ("Ctrl + h: recent history");
            EditorGUILayout.LabelField ("Use t:xxx to search by type");
            EditorGUILayout.LabelField ("Use l:xxx to search by label");
        }
    }
}