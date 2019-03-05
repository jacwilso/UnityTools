using UnityEditor;
using UnityEngine;

namespace QuickFind.Settings {
    public class QuickFindSettings : ScriptableObject {
        // private const string k_SettingsPath = "ProjectSettings/QuickFindSettings.asset"; // TODO find better place
        internal const string k_SettingsPath = "Assets/QuickFind/Editor/QuickFindSettings.asset"; // TODO find better place
        private const EventModifiers k_platformModifierKey =
#if UNITY_EDITOR_OSX
            EventModifiers.Command;
#else
        EventModifiers.Control;
#endif

        [SerializeField]
        private KeyShortcut[] m_shortcuts = new KeyShortcut[] {
            new KeyShortcut ("Open Window", "Opens the window for searching.", k_platformModifierKey, KeyCode.G),
            new KeyShortcut ("History", "History of recent selected objects (window must be open).", k_platformModifierKey, KeyCode.H),
        };
        internal KeyShortcut OpenShortcut => m_shortcuts[0];
        internal KeyShortcut HistoryShortcut => m_shortcuts[1];

        [SerializeField, Tooltip ("Should the type filter, filter as you type (ie. starts with match) or on completion (ie. equals match)?")]
        private bool m_typeFilterStartsWith = false;
        internal bool TypeFilterStartsWith => m_typeFilterStartsWith;
        [SerializeField, Tooltip ("Include all files found in the project or only files found in the 'Assets/' directory.")]
        private bool m_includeProjectFiles = false;
        internal bool IncludeProjectFiles => m_includeProjectFiles;
        [SerializeField, Tooltip ("Should the text in the search field persist across multiple openings of the window?")]
        private bool m_persistentSearch = false;
        internal bool PersistentSearch => m_persistentSearch;

        [SerializeField]
        private WindowProperties m_window = new WindowProperties (600f, 3);
        internal WindowProperties Window => m_window;

        [SerializeField]
        private int m_fontSize;

        internal static QuickFindSettings GetOrCreateSettings () {
            var settings = AssetDatabase.LoadAssetAtPath<QuickFindSettings> (k_SettingsPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<QuickFindSettings> ();
                AssetDatabase.CreateAsset (settings, k_SettingsPath);
                AssetDatabase.SaveAssets ();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings () {
            return new SerializedObject (GetOrCreateSettings ());
        }
    }
}