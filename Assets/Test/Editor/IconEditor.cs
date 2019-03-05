using UnityEditor;
using UnityEngine;

public class IconEditor : EditorWindow {

    [MenuItem ("QuickFind/Icons")]
    private static void ShowWindow () {
        var window = GetWindow<IconEditor> ();
        window.titleContent = new GUIContent ("Icon");
        window.Show ();
    }

    private Vector2 scroll;
    private void OnGUI () {
        scroll = EditorGUILayout.BeginScrollView (scroll);

        EditorGUILayout.EndScrollView ();
    }
}