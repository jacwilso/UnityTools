#if UNITY_2018_3_OR_NEWER
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
            return System.IO.File.Exists (QuickFindSettings.k_SettingsPath);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider () {
            var provider = new SettingsProvider ("Preferences/Quick Find Settings", SettingsScope.User) {
                guiHandler = (searchContext) => {
                        QuickFindPreferences.PreferncesGUI ();
                    },

                    // Populate the search keywords to enable smart search filtering and label highlighting:
                    keywords = new System.Collections.Generic.HashSet<string> (new [] { "open", "open window" }) // TODO finish
            };

            return provider;
        }

        /// USING EXPERIMENTAL API
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
#endif