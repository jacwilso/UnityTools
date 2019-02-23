using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TODO))]
public class TODOEditor : Editor {
    private TODO targetTODO;

    private bool todoFoldout, doneFoldout;

    private void OnEnable () {
        targetTODO = target as TODO;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();

        return;
        var textAlign = new GUIStyle (GUI.skin.textField);
        textAlign.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.BeginHorizontal (EditorStyles.toolbar);
        if (GUILayout.Button ("TODO", EditorStyles.toolbarDropDown)) {
            todoFoldout = !todoFoldout;
        }
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button (EditorGUIUtility.IconContent ("Toolbar Plus"), EditorStyles.toolbarButton)) {
            targetTODO.m_todo.Add (string.Empty);
        }

        EditorGUILayout.EndHorizontal ();
        if (todoFoldout) {
            EditorGUILayout.Space ();
            int len = targetTODO.m_todo.Count;
            for (int i = 0; i < len; i++) {
                EditorGUILayout.BeginHorizontal ();
                targetTODO.m_todo[i] = EditorGUILayout.TextField (targetTODO.m_todo[i], textAlign, GUILayout.Height (20), GUILayout.ExpandWidth (true));
                GUILayout.FlexibleSpace ();
                if (GUILayout.Button (EditorGUIUtility.IconContent ("d_FilterSelectedOnly"), GUIStyle.none)) {
                    targetTODO.m_done.Add (targetTODO.m_todo[i]);
                    targetTODO.m_todo.RemoveAt (i);
                    i--;
                    len--;
                }
                GUILayout.Label ("", GUILayout.Width (5));
                if (GUILayout.Button (EditorGUIUtility.IconContent ("TreeEditor.Trash"), GUIStyle.none)) {
                    targetTODO.m_todo.RemoveAt (i);
                    i--;
                    len--;
                }
                EditorGUILayout.EndHorizontal ();
            }
        }

        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal (EditorStyles.toolbar);
        if (GUILayout.Button ("Done", EditorStyles.toolbarDropDown)) {
            doneFoldout = !doneFoldout;
        }
        GUILayout.FlexibleSpace ();
        EditorGUILayout.EndHorizontal ();
        if (doneFoldout) {
            EditorGUILayout.Space ();
            int len = targetTODO.m_done.Count;
            for (int i = 0; i < len; i++) {
                EditorGUILayout.BeginHorizontal ();
                EditorGUI.BeginDisabledGroup (true);
                EditorGUILayout.LabelField (targetTODO.m_done[i], textAlign);
                EditorGUI.EndDisabledGroup ();
                GUILayout.FlexibleSpace ();
                if (GUILayout.Button (EditorGUIUtility.IconContent ("winbtn_win_close"), GUIStyle.none)) {
                    targetTODO.m_todo.Add (targetTODO.m_done[i]);
                    targetTODO.m_done.RemoveAt (i);
                    i--;
                    len--;
                }
                GUILayout.Label ("", GUILayout.Width (5));
                if (GUILayout.Button (EditorGUIUtility.IconContent ("TreeEditor.Trash"), GUIStyle.none)) {
                    targetTODO.m_done.RemoveAt (i);
                    i--;
                    len--;
                }
                EditorGUILayout.EndHorizontal ();
            }
        }
    }
}