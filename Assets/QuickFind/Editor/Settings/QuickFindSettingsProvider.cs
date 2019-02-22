using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
// using UnityEditor.Experimental.UIElements;
// using UnityEngine.Experimental.UIElements;
// using UnityEngine.Experimental.UIElements.StyleEnums;

namespace QuickFind.Settings {
    public class QuickFindSettingsProvider : SettingsProvider {
        private SerializedObject m_Settings;

        public QuickFindSettingsProvider (string path, SettingsScope scope) : base (path, scope) { }

        public static bool IsSettingsAvailable () {
            return File.Exists (QuickFindSettings.k_SettingsPath);
        }

        private static bool keySelect = false;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider () {
            var provider = new SettingsProvider ("Preferences/Plugins/Quick Find Settings", SettingsScope.User) {
                guiHandler = (searchContext) => {
                        var settings = QuickFindSettings.GetSerializedSettings ();

                        // Key Shortcuts
                        {
                            var keyProp = settings.FindProperty ("m_openKey");
                            var modProp = settings.FindProperty ("m_openModifier");
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

                            EditorGUILayout.LabelField ("Open Shortcut", EditorStyles.boldLabel);
                            EditorGUILayout.BeginHorizontal ();
                            EditorGUILayout.LabelField ("Key:");
                            if (GUILayout.Button (keySelect? "[Enter a key]": openKeyCombo == null? "None": openKeyCombo.ToString (), GUI.skin.textField)) {
                                keySelect = true;
                            }
                            if (GUILayout.Button ("Clear") || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)) {
                                keySelect = false;
                                Debug.Log (key);
                                keyProp.intValue = 0;
                            }
                            EditorGUILayout.EndHorizontal ();

                            EditorGUILayout.BeginHorizontal ();
                            EditorGUILayout.LabelField ("Modifiers:");
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

                            if (keySelect && Event.current.type == EventType.KeyDown) {
                                keySelect = false;
                                keyProp.intValue = (int) Event.current.keyCode;
                                Debug.Log (Event.current.keyCode + " " + keyProp.intValue);
                                modProp.intValue = (int) Event.current.modifiers;
                                Event.current.Use ();
                            }
                        }

                        {
                            EditorGUILayout.PropertyField (settings.FindProperty ("m_typeFilterStartsWith"));
                            EditorGUILayout.PropertyField (settings.FindProperty ("m_includeProjectFiles"));
                        }

                        settings.ApplyModifiedProperties ();
                    },

                    // Populate the search keywords to enable smart search filtering and label highlighting:
                    keywords = new HashSet<string> (new [] { "open", "open window" })
            };

            return provider;
        }

        // class Styles {
        //     public static GUIContent openWindow = new GUIContent ("Open Window");
        // }
        // [SettingsProvider]
        // public static SettingsProvider CreateSettingsProvider () {
        //     if (IsSettingsAvailable ()) {
        //         var provider = new QuickFindSettingsProvider ("QuickFindSettings", SettingsScope.User);

        //         provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles> ();
        //         return provider;
        //     }

        //     return null;
        // }

        // public override void OnActivate (string searchContext, VisualElement rootElement) {
        //     m_Settings = QuickFindSettings.GetSerializedSettings ();
        // }
        // [SettingsProvider]
        // public static SettingsProvider CreateSettingsProvider () {
        //     var provider = new SettingsProvider ("Preferences/Plugins/QuickFindSettings", SettingsScope.User) {
        //         label = "Quick Find Settings",
        //             activateHandler = (searchContext, rootElement) => {
        //                 var settings = QuickFindSettings.GetSerializedSettings ();
        //                 rootElement.AddStyleSheetPath ("Assets/Editor/settings_ui.uss");
        //                 var title = new Label () {
        //                     text = "Quick Find Settings"
        //                 };
        //                 title.AddToClassList ("title");
        //                 rootElement.Add (title);

        //                 const string propertyString = "property-list";
        //                 var properties = new VisualElement () {
        //                     style = {
        //                     flexDirection = FlexDirection.Column
        //                     }
        //                 };
        //                 properties.AddToClassList (propertyString);
        //                 rootElement.Add (properties);

        //                 {
        //                     var openProperties = new VisualElement () {
        //                         style = {
        //                         flexDirection = FlexDirection.Row,
        //                         width = 300f,
        //                         }
        //                     };
        //                     openProperties.AddToClassList (propertyString);
        //                     rootElement.Add (openProperties);

        //                     var label = new Label (
        //                         "Open Command"
        //                     );
        //                     label.AddToClassList (propertyString);
        //                     openProperties.Add (label);

        //                     var modField = new EnumField (
        //                         (EventModifiers) settings.FindProperty ("m_openModifier").intValue
        //                     );
        //                     modField.AddToClassList (propertyString);
        //                     openProperties.Add (modField);

        //                     var keyField = new EnumField (
        //                         (KeyCode) settings.FindProperty ("m_openKey").intValue
        //                     );
        //                     keyField.AddToClassList (propertyString);
        //                     openProperties.Add (keyField);
        //                 }

        //                 // var textField = new TextField () {
        //                 // value = settings.FindProperty ("m_openModifier").stringValue
        //                 // };

        //             },
        //             keywords = new HashSet<string> (new [] { "m_openModifier", "m_openKey" })
        //     };

        //     return provider;
        // }
    }
}