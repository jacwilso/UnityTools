using UnityEditor;
using UnityEngine;

namespace QuickFind.Editor {
    [InitializeOnLoad]
    internal static class QuickFindWindowListener {
        private static QuickFind.Settings.QuickFindSettings settings;

        static QuickFindWindowListener () {
            System.Reflection.FieldInfo globalEventHandlerField = typeof (EditorApplication).GetField ("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            EditorApplication.CallbackFunction globalEventHandler = (EditorApplication.CallbackFunction) globalEventHandlerField.GetValue (null);

            globalEventHandler += OnGlobalEvent;
            globalEventHandlerField.SetValue (null, globalEventHandler);
            // SceneView.onSceneGUIDelegate += OnSceneGUI;

            settings = QuickFind.Settings.QuickFindSettings.GetOrCreateSettings ();
        }

        // private static void OnSceneGUI (SceneView sceneView) {
        private static void OnGlobalEvent () {
            if (Event.current.type == EventType.KeyDown &&
                Event.current.modifiers == settings.OpenShortcut.Modifiers &&
                Event.current.keyCode == settings.OpenShortcut.Key) {
                QuickFindWindow.ShowWindow ();
            }
        }
    }
}