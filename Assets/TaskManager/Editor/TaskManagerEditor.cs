using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskManager.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TaskManager {
    public class TaskManagerEditor : EditorWindow {
        private Vector2 scroll;

        // TOOLBAR
        private SearchField searchField;
        private string searchText;

        private bool isRenaming;
        private bool setFocus;
        private int controlID = -1;

        private bool tagMaskOpen;
        private int selectedTags;
        public static HashSet<string> tags = new HashSet<string> () {
            "New"
        };

        private int selectedBoard = -1;
        private List<Board> boards;

        private Rect cardDragRect;
        private int focusedCard = -1;

        public class Styles {
            public readonly string proPrefix = EditorGUIUtility.isProSkin? "d_": string.Empty;
            public readonly GUIStyle centerLabel = new GUIStyle (GUI.skin.label);

            public readonly GUIStyle cardWindow = new GUIStyle (GUI.skin.box);
            public readonly GUIStyle moveArrow = new GUIStyle ();
            public readonly GUIStyle cardBox = new GUIStyle (GUI.skin.box);

            public Styles () {
                centerLabel.alignment = TextAnchor.UpperCenter;

                moveArrow.margin.top = 15;
                cardBox.margin.left = cardBox.margin.right =
                    moveArrow.margin.left = moveArrow.margin.right = 0;
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

        [MenuItem ("TaskManager/Task Manager")]
        private static void ShowWindow () {
            var window = GetWindow<TaskManagerEditor> ();
            window.titleContent = new GUIContent ("Task Manager");
            window.Show ();
        }

        private void OnEnable () {
            var boardAssets = AssetDatabase.FindAssets ("t:Board");
            boards = new List<Board> (boardAssets.Length);
            foreach (var guid in boardAssets) {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                boards.Add (AssetDatabase.LoadMainAssetAtPath (path) as Board);
            }

            searchField = new SearchField ();
            controlID = -1;
        }

        private void OnGUI () {
            Toolbar ();

            BoardGUI ();
        }

        private void Toolbar () {
            EditorGUILayout.BeginHorizontal (EditorStyles.toolbar);
            //TimelineSelector
            EditorGUI.BeginDisabledGroup (boards.Count == 0);
            if (!isRenaming) {
                selectedBoard = EditorGUILayout.Popup (selectedBoard, boards.Select (x => x.title).ToArray (), EditorStyles.toolbarPopup);
            } else {
                EditorGUI.BeginChangeCheck ();
                GUI.SetNextControlName ("RenameTextField");
                boards[selectedBoard].title = EditorGUILayout.DelayedTextField (boards[selectedBoard].title, EditorStyles.toolbarPopup);
                if (EditorGUI.EndChangeCheck ()) {
                    isRenaming = false;
                }
                if (setFocus) {
                    EditorGUI.FocusTextInControl ("RenameTextField");
                    setFocus = false;
                }
            }
            EditorGUI.EndDisabledGroup ();
            EditorGUI.BeginDisabledGroup (selectedBoard == -1);
            if (!isRenaming) {
                if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "editicon.sml", "|Rename board"), EditorStyles.toolbarButton)) {
                    isRenaming = setFocus = true;
                }
            } else {
                if (GUILayout.Button (EditorGUIUtility.IconContent ("d_FilterSelectedOnly", "|Done renaming"), EditorStyles.toolbarButton)) {
                    isRenaming = false;
                }
            }
            if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "TreeEditor.Trash", "|Delete board"), EditorStyles.toolbarButton)) {
                AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath (boards[selectedBoard]));
                boards.RemoveAt (selectedBoard);
                selectedBoard = -1;
            }
            if (GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "Toolbar Plus", "|Add card"), EditorStyles.toolbarButton)) {
                CreateCard ();
            }
            EditorGUI.EndDisabledGroup ();
            GUILayout.FlexibleSpace ();
            searchField.OnToolbarGUI (searchText);
            tagMaskOpen = EditorGUILayout.DropdownButton (EditorGUIUtility.IconContent (m_styles.proPrefix + "FilterByLabel"), FocusType.Passive, EditorStyles.toolbarDropDown);
            if (tagMaskOpen) {
                selectedTags = EditorGUILayout.MaskField (selectedTags, tags.ToArray (), EditorStyles.toolbarPopup);
            }
            // FilterByType
            // Preset Icon could be used for settings
            EditorGUILayout.EndHorizontal ();
        }

        private void BoardGUI () {
            if (selectedBoard < 0) {
                BoardsEmpty ();
            } else {
                scroll = EditorGUILayout.BeginScrollView (scroll);
                BeginWindows ();
                var winRect = new Rect (10, 10, 200f, 300f);
                int len = boards[selectedBoard].cards.Count ();
                for (int i = 0; i < len; i++) {
                    var rect = focusedCard == i? cardDragRect : winRect;
                    rect = GUI.Window (i, rect, CardWindowGUI, "", GUI.skin.box);
                    if (focusedCard == i) {
                        cardDragRect = rect;
                    }
                    winRect.x += winRect.width + 10;
                }
                EndWindows ();
                EditorGUILayout.EndScrollView ();

                // int j = (int) (cardDragRect.x / 210f);
                // Debug.Log ("focused " + focusedCard + " shift to " + j);
                // if (j != focusedCard && j < len) {
                //     var tmp = boards[selectedBoard].cards[focusedCard];
                //     boards[selectedBoard].cards[focusedCard] = boards[selectedBoard].cards[j];
                //     boards[selectedBoard].cards[j] = tmp;
                //     focusedCard = j;
                // }
            }
        }

        private void CardWindowGUI (int id) {
            if (Event.current.type == EventType.MouseDown) {
                focusedCard = id;
                cardDragRect = new Rect (id * 210f + 10f, 10, 200f, 300f);
            }

            EditorGUILayout.Space ();
            boards[selectedBoard].cards[id].title = EditorGUILayout.TextField (boards[selectedBoard].cards[id].title, m_styles.centerLabel);
            EditorGUILayout.Space ();

            int cardLen = boards[selectedBoard].cards.Count;
            var tasks = boards[selectedBoard].cards[id].tasks;
            int len = tasks.Count;
            for (int i = 0; i < len; i++) {
                GUILayout.BeginHorizontal ();
                if (id > 0 && GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "tab_prev@2x", "|Quick move to previous card"), m_styles.moveArrow)) {
                    boards[selectedBoard].cards[id - 1].tasks.Add (tasks[i]);
                    tasks.RemoveAt (i);
                    i--;
                    len--;
                    continue;
                }
                GUILayout.FlexibleSpace ();
                if (GUILayout.Button (tasks[i].title, m_styles.cardBox, GUILayout.Width (160), GUILayout.Height (40f))) {
                    TaskEditor.ShowWindow (tasks[i], boards[selectedBoard].cards[id], this);
                }
                GUILayout.FlexibleSpace ();
                if (id < cardLen - 1 && GUILayout.Button (EditorGUIUtility.IconContent (m_styles.proPrefix + "tab_next@2x", "|Quick move to next card"), m_styles.moveArrow)) {
                    boards[selectedBoard].cards[id + 1].tasks.Add (tasks[i]);
                    tasks.RemoveAt (i);
                    i--;
                    len--;
                    continue;
                }
                GUILayout.EndHorizontal ();
                // if (Event.current.type == EventType.MouseUp) {

                // }
            }

            GUILayout.FlexibleSpace ();
            var content = new GUIContent (EditorGUIUtility.IconContent (m_styles.proPrefix + "Toolbar Plus"));
            content.text = "Task";
            if (GUILayout.Button (content)) {
                boards[selectedBoard].cards[id].tasks.Add (new Task ());
            }
            EditorGUILayout.Space ();
            GUI.DragWindow ();
        }

        private void BoardsEmpty () {
            GUILayout.FlexibleSpace ();

            EditorGUILayout.BeginHorizontal ();
            GUILayout.FlexibleSpace ();
            EditorGUILayout.LabelField ("To get started create a board.", m_styles.centerLabel);
            GUILayout.FlexibleSpace ();
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            GUILayout.FlexibleSpace ();
            if (GUILayout.Button ("Create", GUILayout.Width (50))) {
                CreateBoard ();
            }
            GUILayout.FlexibleSpace ();
            EditorGUILayout.EndHorizontal ();

            GUILayout.FlexibleSpace ();
        }

        private void CreateBoard () {
            var path = EditorUtility.SaveFilePanelInProject ("Create Board", "Board Test", "asset", "Enter the title of your board:");
            if (!string.IsNullOrEmpty (path)) {
                var board = ScriptableObject.CreateInstance<Board> ();
                board.title = Path.GetFileNameWithoutExtension (path);
                AssetDatabase.CreateAsset (board, path);
                selectedBoard = boards.Count ();
                boards.Add (board);
            } else {
                // ERROR
            }
        }

        private void CreateCard () {
            boards[selectedBoard].cards.Add (new Card ());
        }
    }
}