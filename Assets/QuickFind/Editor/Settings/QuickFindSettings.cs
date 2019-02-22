using UnityEditor;
using UnityEngine;

namespace QuickFind.Settings {
    public class QuickFindSettings : ScriptableObject {
        public const string k_SettingsPath = "Assets/Editor/QuickFindSettings.asset";

        //DATA
        [SerializeField]
        private EventModifiers m_openModifier;
        [SerializeField]
        private KeyCode m_openKey;

        [SerializeField, Tooltip ("Should the type filter, filter as you type (ie. starts with match) or on completion (ie. equals match)?")]
        private bool m_typeFilterStartsWith;
        public bool TypeFilterStartsWith => m_typeFilterStartsWith;
        [SerializeField, Tooltip ("Include all files found in the project or only files found in the Assets/ directory.")]
        private bool m_includeProjectFiles;
        public bool IncludeProjectFiles => m_includeProjectFiles;

        internal static QuickFindSettings GetOrCreateSettings () {
            var settings = AssetDatabase.LoadAssetAtPath<QuickFindSettings> (k_SettingsPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<QuickFindSettings> ();
                //DATA
                AssetDatabase.CreateAsset (settings, k_SettingsPath);
                AssetDatabase.SaveAssets ();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings () {
            return new SerializedObject (GetOrCreateSettings ());
        }

        // TODO possible solution https://answers.unity.com/questions/10582/customize-shortcuts-in-the-unity-editor.html
        //         public static string MenuItemShortcut () {
        //             var settings = QuickFindSettings.GetSerializedSettings ();
        //             var key = (KeyCode) settings.FindProperty ("m_openKey").intValue;
        //             var modKeys = (EventModifiers) settings.FindProperty ("m_openModifier").intValue;

        //             StringBuilder openKeyCombo = null;
        //             if (key != 0) {
        //                 openKeyCombo = new StringBuilder (key.ToString ());
        //             }
        //             bool alt = modKeys.HasFlag (EventModifiers.Alt);
        //             if (alt) {
        //                 openKeyCombo?.Insert (0, "&");
        //             }
        //             bool shift = modKeys.HasFlag (EventModifiers.Shift);
        //             if (shift) {
        //                 openKeyCombo?.Insert (0, "#");
        //             }
        // #if UNITY_EDITOR_OSX
        //             bool control = modKeys.HasFlag (EventModifiers.Command);
        //             if (control) {
        //                 openKeyCombo?.Insert (0, "#");
        //             }
        // #else
        //             bool control = modKeys.HasFlag (EventModifiers.Control);
        //             if (control) {
        //                 openKeyCombo?.Insert (0, "#");
        //             }
        // #endif
        //             return openKeyCombo == null? "": openKeyCombo.ToString ();
        //         }
    }
}