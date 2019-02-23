using TaskManager.Data;
using UnityEditor;
using UnityEngine;

namespace TaskManager {
    public class TaskEditor : EditorWindow {
        private static Task target;
        private static Card parent;
        private static TaskManagerEditor editor;

        private bool subTasksFoldout;
        private bool assetsFoldout;
        private int tagMask = -1;

        public class Styles {
            public readonly string proPrefix = EditorGUIUtility.isProSkin? "d_": string.Empty;

            public Styles () {

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

        public static void ShowWindow (Task task, Card card, TaskManagerEditor taskManager) {
            var window = GetWindow<TaskEditor> ();
            window.titleContent = new GUIContent ("Task");
            window.Show ();

            target = task;
            parent = card;
            editor = taskManager;
        }

        private void OnGUI () {
            if (target == null) {
                Close ();
            }

            Toolbar ();
            EditorGUILayout.Space ();

            EditorGUILayout.LabelField ("Task");
            EditorGUI.BeginChangeCheck ();
            target.title = EditorGUILayout.TextField (target.title);
            if (EditorGUI.EndChangeCheck ()) {
                editor.Repaint ();
            }

            EditorGUILayout.Space ();

            EditorGUILayout.LabelField ("Description");
            target.description = EditorGUILayout.TextArea (target.description, GUILayout.Height (180f));

            EditorGUILayout.BeginHorizontal ();
            var assetContent = new GUIContent (EditorGUIUtility.IconContent ("BillboardAsset Icon", "|Related assets"));
            assetContent.text = "Assets";
            if (GUILayout.Button (assetContent)) {

            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            subTasksFoldout = EditorGUILayout.Foldout (subTasksFoldout, "Subtasks");

            GUILayout.FlexibleSpace ();
            if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "Toolbar Plus"))) {
                target.subTasks.Add (new SubTask ());
            }

            EditorGUILayout.EndHorizontal ();

            if (target.subTasks.Count > 0 && subTasksFoldout) {
                SubTaskGUI ();
            }

            // GUILayout.FlexibleSpace ();
            // var content = new GUIContent (EditorGUIUtility.IconContent (m_styles.proPrefix + "Toolbar Plus"));
            // content.text = "Task";
            // if (GUILayout.Button (content)) {
            //     target.subTasks.Add (new SubTask ());
            // }
            // EditorGUILayout.Space ();
        }

        private void Toolbar () {
            EditorGUILayout.BeginHorizontal (EditorStyles.toolbar);
            var tagContent = new GUIContent (EditorGUIUtility.IconContent ("FilterByLabel"));
            tagContent.text = "Tags";
            // EditorGUILayout.MaskField (tagContent, tagMask, TaskManager.tags, )
            // if (GUILayout.DropdownButton (tagContent, EditorStyles.toolbarDropDown)) {
            // PopupWindow.Show(GUILayoutUtility.GetLastRect(), )
            // }

            // var colorContent = new GUIContent (EditorGUIUtility.IconContent ("BillboardAsset Icon", "|Related assets"));
            // assetContent.text = "Colors";

            GUILayout.FlexibleSpace ();
            if (EditorGUILayout.DropdownButton (EditorGUIUtility.IconContent (m_styles.proPrefix + "Collab.FolderMoved", "|Move"), FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width (30))) {

            }
            if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "TreeEditor.Trash"), EditorStyles.toolbarButton, GUILayout.Width (30))) {
                parent.tasks.Remove (target);
                editor.Repaint ();
                target = null;
                Close ();
                return;
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void SubTaskGUI () {
            var progressRect = EditorGUILayout.GetControlRect ();
            GUILayout.Space (5);
            int len = target.subTasks.Count;
            int completed = 0;
            for (int i = 0; i < len; i++) {
                var sub = target.subTasks[i];
                EditorGUILayout.BeginHorizontal (GUI.skin.box);
                sub.isComplete = EditorGUILayout.Toggle (sub.isComplete);
                sub.task = EditorGUILayout.TextField (sub.task);
                if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "TreeEditor.Trash", "|Delete subtask"), GUIStyle.none)) {
                    target.subTasks.RemoveAt (i);
                    i--;
                    len--;
                } else {
                    completed += sub.isComplete? 1 : 0;
                }
                EditorGUILayout.EndHorizontal ();
            }
            progressRect.xMin += GUI.skin.textArea.margin.left + 1;
            progressRect.xMax -= GUI.skin.textArea.margin.right - 1;
            progressRect.height = 20f;
            float progress = (float) completed / target.subTasks.Count;
            EditorGUI.ProgressBar (progressRect, progress, string.Format ("{0:N0}%", 100f * progress));
        }
    }
}