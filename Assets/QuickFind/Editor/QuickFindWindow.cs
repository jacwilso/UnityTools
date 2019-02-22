using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace QuickFind.Editor {
    public class QuickFindEditor : EditorWindow {
        // private static TrieNode root = new TrieNode ();

        private string searchField;
        private bool searchWantsFocus;
        private int controlId;
        private int prevCursorPos = 0, prevSelectCursorPos = 0;

        private List<Searchable.ISearchable> matching;
        private int selectedResult = -1;
        private Vector2 matchingScroll;

        private static bool initialize;
        private Texture2D searchIcon, cancelIcon;

        public class Styles {
            public readonly GUIStyle searchFieldBg = new GUIStyle (GUI.skin.textField);
            public readonly GUIStyle searchFieldOverlay = new GUIStyle (GUIStyle.none);
            public readonly float searchFieldHeight;

            public readonly GUIStyle resultRowDefault = new GUIStyle (GUIStyle.none);
            public readonly GUIStyle resultRowSelected = new GUIStyle (GUIStyle.none);
            public readonly GUIStyle resultCommand = new GUIStyle (GUI.skin.label);
            public readonly GUIStyle resultDescription = new GUIStyle (GUI.skin.label);

            private readonly Color resultSelected = new Color32 (61, 128, 223, 230);
            private int searchFontSize = 30;
            private int resultCommandSize = 15;
            private int resultDescriptionSize = 10; // TODO customizable?

            public Styles () {
                searchFieldBg.fontSize = searchFontSize;
                searchFieldOverlay.fontSize = searchFieldBg.fontSize;
                searchFieldHeight = searchFieldBg.CalcHeight (new GUIContent (), 0) + searchFieldBg.margin.vertical;

                resultCommand.fontSize = resultCommandSize;
                resultDescription.fontSize = resultDescriptionSize;

                var tex = new Texture2D (1, 1);
                for (int y = 0; y < 1; y++) {
                    for (int x = 0; x < 1; x++) {
                        tex.SetPixel (x, y, resultSelected);
                    }
                }
                tex.Apply ();
                resultRowSelected.normal.background = tex;
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

        [MenuItem ("QuickFind/QuickFind %g")]
        private static void ShowWindow () {
            var window = ScriptableObject.CreateInstance (typeof (QuickFindEditor)) as QuickFindEditor;
            window.maxSize = new Vector2 (600f, 200f);
            window.CenterOnApplicationWindow (window.maxSize);
            var winRect = window.position;
            winRect.height = m_styles.searchFieldHeight;
            window.position = winRect;

            window.ShowPopup ();
        }

        [MenuItem ("QuickFind/Init %c")]
        private static void Init () {

        }

        private void Awake () {
            QuickFindCache.Awake ();
        }

        private void OnEnable () {
            if (!initialize) { // todo get rid
                Init ();
            }
            matching = new List<Searchable.ISearchable> ();

            // icon = EditorGUIUtility.Load ("d_viewtoolzoom.png") as Texture2D;
            searchIcon = EditorGUIUtility.Load ("lookdevenvrotation.png") as Texture2D;
            // icon = EditorGUIUtility.Load ("tab_next.png") as Texture2D;
            // icon = EditorGUIUtility.Load ("aboutwindow.mainheader") as Texture2D;
            // icon = Resources.Load<Texture2D> ("search_icon");

            cancelIcon = EditorGUIUtility.Load ("icons/lookdevclose.png") as Texture2D;
            //ViewToolZoom
            //winbtn_win_close
            //WaitSpin00

            searchWantsFocus = true;
        }

        private void OnDisable () {
            // GUIUtility.keyboardControl = -1;
        }

        private void OnFocus () { }

        private void OnLostFocus () {
            this.Close ();
            matching.Clear ();
            searchField = string.Empty;
            matchingScroll = Vector2.zero;
        }

        private void OnDestroy () { }

        private void OnGUI () {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
                this.Close ();
                matching.Clear ();
                searchField = string.Empty;
                matchingScroll = Vector2.zero;
            }

            SearchFieldGUI ();
            HandleSearchFieldCursor ();
            MatchingScrollGUI ();

            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) {
                if (string.IsNullOrEmpty (searchField)) {
                    var winRect = position;
                    winRect.height = m_styles.searchFieldHeight;
                    position = winRect;
                } else {
                    var winRect = position;
                    winRect.height = 200f;
                    position = winRect;
                }
            }
        }

        private void SearchFieldGUI () {
            GUILayout.Label ("", m_styles.searchFieldBg);
            var searchRect = GUILayoutUtility.GetLastRect ();

            searchRect.xMin += m_styles.searchFieldBg.padding.left;
            searchRect.xMax -= m_styles.searchFieldBg.padding.right; {
                GUI.BeginGroup (searchRect);
                // Search Icon
                var rect = searchRect;
                rect.position = Vector2.zero;
                rect.width = searchIcon.width;
                GUI.DrawTexture (rect, searchIcon, ScaleMode.ScaleToFit, true);

                // Search Field
                rect.x += rect.width + m_styles.searchFieldBg.padding.right;
                rect.width = searchRect.width - searchIcon.width - cancelIcon.width - m_styles.searchFieldBg.padding.horizontal;
                controlId = GUIUtility.GetControlID (FocusType.Keyboard) + 1;
                var prevSearch = searchField;
                searchField = GUI.TextField (rect, searchField, m_styles.searchFieldOverlay);

                if (!string.IsNullOrEmpty (searchField) && !searchField.Equals (prevSearch)) {
                    var matchString = searchField;
                    selectedResult = -1;
                    matching.Clear ();
                    var assetPaths = QuickFindCache.AssetPaths;

                    int colonIndex = matchString.IndexOf (':');
                    int len = matchString.Length - 1;
                    List<Func<string, bool>> filterFuncs = new List<Func<string, bool>> ();
                    while (colonIndex > 0 && colonIndex < len) {
                        char filterType = matchString[colonIndex - 1];
                        var filterEnd = matchString.IndexOf (' ', colonIndex);
                        string filter;
                        if (filterEnd == -1) {
                            filter = matchString.Substring (colonIndex + 1);
                            matchString = matchString.Remove (colonIndex - 1, matchString.Length - (colonIndex - 1));
                        } else {
                            filter = matchString.Substring (colonIndex + 1, filterEnd - (colonIndex + 1));
                            matchString = matchString.Remove (colonIndex - 1, filterEnd + 1);
                        }
                        if (string.IsNullOrEmpty (filter)) {
                            continue;
                        }
                        filterFuncs.Add (QuickFindUtility.Filter (filterType, filter));

                        colonIndex = matchString.IndexOf (':');
                    }

                    assetPaths = assetPaths
                        .Where (x => {
                            bool include = x.Contains (matchString, StringComparison.OrdinalIgnoreCase);
                            int i = 0;
                            int filterLen = filterFuncs.Count;
                            while (include && i < filterLen) {
                                include = filterFuncs[i++] (x);
                            }
                            return include;
                        });
                    foreach (var path in assetPaths) {
                        matching.Add (new Searchable.Asset (path));
                    }

                    var menuItems = QuickFindCache.MenuItems
                        .Where (x => x.Key.Contains (searchField, StringComparison.OrdinalIgnoreCase));
                    foreach (var item in menuItems) {
                        matching.Add (new Searchable.MenuItemCommand (item.Key, item.Value));
                    }
                }
                rect.x += rect.width;
                rect.width = cancelIcon.width;
                GUI.DrawTexture (rect, cancelIcon, ScaleMode.ScaleToFit, true);
                GUI.EndGroup ();

                if (searchWantsFocus) {
                    GUIUtility.keyboardControl = controlId;
                    EditorGUIUtility.editingTextField = true;
                }
            }
        }

        private void HandleSearchFieldCursor () {
            TextEditor textEd = GUIUtility.GetStateObject (typeof (TextEditor), GUIUtility.keyboardControl) as TextEditor;
            if (searchWantsFocus) {
                searchWantsFocus = false;
                GUIUtility.keyboardControl = controlId;
                textEd = (TextEditor) GUIUtility.GetStateObject (typeof (TextEditor), GUIUtility.keyboardControl);
                if (textEd != null) {
                    textEd.SelectNone ();
                    // textEd.MoveTextEnd ();
                }
            }
            if (textEd != null) {
                prevCursorPos = textEd.cursorIndex;
                prevSelectCursorPos = textEd.selectIndex;
            }

            if (Event.current.keyCode == KeyCode.DownArrow || Event.current.keyCode == KeyCode.UpArrow) {
                // textEd.SelectAll ();
                textEd.MoveTextEnd ();
                if (Event.current.type == EventType.Used) {
                    selectedResult += 2 * (Event.current.keyCode - KeyCode.UpArrow) - 1;
                    if (selectedResult < 0) {
                        selectedResult = -1;
                    }
                    // TODO Move the scroll position
                }
                // Event.current.Use ();
            } else if (Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t') {
                searchWantsFocus = true;
                Event.current.Use ();
            } else if (selectedResult != -1 && Event.current.keyCode == KeyCode.Return) {
                matching[selectedResult].Execute ();
            }
        }

        private void MatchingScrollGUI () {
            matchingScroll = EditorGUILayout.BeginScrollView (matchingScroll, GUILayout.Width (0.5f * position.width));
            int len = matching.Count;
            for (int i = 0; i < len; i++) {
                MatchingItem (matching[i], selectedResult == i, i);
            }
            EditorGUILayout.EndScrollView ();
        }

        private void MatchingItem (Searchable.ISearchable item, bool selected, int i) {
            if (selected) {
                EditorGUILayout.BeginHorizontal (m_styles.resultRowSelected);
            } else {
                EditorGUILayout.BeginHorizontal (m_styles.resultRowDefault); // TODO GUIStyle.none doesn't work for some reason??
            }
            // if (i < 3) { // figure out only on screen
            // GUIContent content = EditorGUIUtility.ObjectContent (AssetDatabase.LoadMainAssetAtPath (matching[i]), AssetDatabase.GetMainAssetTypeAtPath (matching[i])); // TODO Cache?
            // var content = AssetPreview.GetMiniTypeThumbnail (AssetDatabase.GetMainAssetTypeAtPath (matching[i]));
            // GUILayout.Label (content);
            // }
            // EditorGUIUtility.TrIconContent ("Tooolbar Minus");
            EditorGUILayout.BeginVertical ();
            GUILayout.Label (item.Command, m_styles.resultCommand);
            GUILayout.Label (item.Description, m_styles.resultDescription);
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();

            var labelRect = GUILayoutUtility.GetLastRect ();
            if (Event.current.type == EventType.MouseDown && labelRect.Contains (Event.current.mousePosition)) {
                if (selected || Event.current.clickCount == 2) {
                    selectedResult = i;
                    item.Execute ();
                } else {
                    selectedResult = i;
                }
                Event.current.Use ();
            }
        }
    }
}