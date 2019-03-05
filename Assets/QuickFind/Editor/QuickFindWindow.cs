using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace QuickFind.Editor {
    public class QuickFindWindow : EditorWindow {
        // private static TrieNode root = new TrieNode ();

        private string searchField;
        private bool searchWantsFocus;
        private int controlId;
        private int prevCursorPos = 0, prevSelectCursorPos = 0;
        private bool expandWindow;

        private List<Searchable.ISearchable> matching;
        // private Stack<List<Searchable.ISearchable>> matchStack;
        private int selectedResult = -1;
        private Vector2 matchingScroll;

        private Action<string> OnSearch;

        private static QuickFind.Settings.QuickFindSettings settings;

        public class Styles {
            public readonly GUIStyle searchFieldBg = new GUIStyle (GUI.skin.textField);
            public readonly GUIStyle searchFieldOverlay = new GUIStyle (GUIStyle.none);
            public readonly float searchFieldHeight;
            public readonly GUIContent searchIcon = EditorGUIUtility.IconContent ("LookDevEnvRotation@2x");

            public readonly GUIStyle resultRowDefault = new GUIStyle (GUIStyle.none);
            public readonly GUIStyle resultRowSelected = new GUIStyle (GUIStyle.none);
            public readonly GUIStyle resultCommand = new GUIStyle (GUI.skin.label);
            public readonly GUIStyle resultDescription = new GUIStyle (GUI.skin.label);

            public readonly GUIStyle iconBtn = new GUIStyle (EditorStyles.boldLabel);
            public readonly GUIContent favoriteIcon = EditorGUIUtility.IconContent ("Favorite Icon");
            public readonly GUIContent historyIcon = EditorGUIUtility.IconContent ("UnityEditor.AnimationWindow"); // TODO better icon

            private readonly Color resultSelected = new Color32 (61, 128, 223, 230);
            private int searchFontSize = 30;
            private int resultCommandSize = 15;
            private int resultDescriptionSize = 10; // TODO customizable?

            private readonly Color iconBtnClick = new Color32 (40, 40, 40, 80);
            private int iconBtnFontSize = 12;
            public readonly float iconSize = 40;

            public Styles () {
                searchFieldBg.fontSize = searchFontSize;
                searchFieldOverlay.fontSize = searchFieldBg.fontSize;
                searchFieldHeight = searchFieldBg.CalcHeight (new GUIContent (), 0) + searchFieldBg.margin.vertical;

                resultCommand.fontSize = resultCommandSize;
                resultDescription.fontSize = resultDescriptionSize;
                resultRowSelected.normal.background = GenerateTex (resultSelected);

                iconBtn.fontSize = iconBtnFontSize;
                iconBtn.alignment = TextAnchor.MiddleLeft;
                iconBtn.active.background = GenerateTex (iconBtnClick);

                favoriteIcon.text = "Favorites";
                historyIcon.text = "History";
            }

            private Texture2D GenerateTex (Color c) {
                var tex = new Texture2D (1, 1);
                for (int y = 0; y < 1; y++) {
                    for (int x = 0; x < 1; x++) {
                        tex.SetPixel (x, y, c);
                    }
                }
                tex.Apply ();
                return tex;
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

        [MenuItem ("QuickFind/QuickFind")]
        internal static void ShowWindow () {
            var window = ScriptableObject.CreateInstance (typeof (QuickFindWindow)) as QuickFindWindow;
            if (settings == null) {
                settings = QuickFind.Settings.QuickFindSettings.GetOrCreateSettings ();
            }

            window.maxSize = new Vector2 (600f, m_styles.searchFieldHeight + m_styles.iconSize * settings.Window.ExpandedRows); // TODO fix (this is too small)
            window.minSize = new Vector2 (600f, m_styles.searchFieldHeight);
            window.CenterOnApplicationWindow (window.maxSize);
            var winRect = window.position;
            winRect.height = m_styles.searchFieldHeight;
            window.position = winRect;

            window.ShowPopup ();
        }

        private void OnEnable () {
            matching = new List<Searchable.ISearchable> ();
            // matchStack = new Stack<List<Searchable.ISearchable>> ();
            searchWantsFocus = true;
            OnSearch = DefaultSearch;
        }

        private void OnDisable () {
            matching.Clear ();
            searchField = string.Empty;
            matchingScroll = Vector2.zero;
        }

        private void OnFocus () { }

        private void OnLostFocus () {
            Close ();
        }

        private void OnDestroy () { }

        private void OnGUI () {
            HandleInputEvents ();

            SearchFieldGUI ();
            HandleSearchFieldCursor ();

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical ();
            IconButtonGUI ();
            EditorGUILayout.EndVertical ();

            MatchingScrollGUI ();
            EditorGUILayout.EndHorizontal ();

            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) {
                if (!expandWindow && string.IsNullOrEmpty (searchField)) {
                    matching.Clear ();
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
                var rect = new Rect (0, 0.3f * m_styles.searchIcon.image.height, m_styles.searchIcon.image.width, searchRect.height);
                EditorGUIUtility.AddCursorRect (rect, MouseCursor.Link);
                if (GUI.Button (rect, m_styles.searchIcon, GUIStyle.none)) {
                    expandWindow = !expandWindow;
                }

                // Search Field
                rect.x += rect.width + m_styles.searchFieldBg.padding.right;
                rect.y = searchRect.y - 1;
                rect.width = searchRect.width - rect.width - m_styles.searchFieldBg.padding.horizontal;
                EditorGUI.BeginChangeCheck ();
                controlId = GUIUtility.GetControlID (FocusType.Keyboard) + 1;
                var prevSearch = searchField;
                searchField = GUI.TextField (rect, searchField, m_styles.searchFieldOverlay);
                if (EditorGUI.EndChangeCheck () &&
                    Event.current.keyCode != KeyCode.DownArrow &&
                    Event.current.keyCode != KeyCode.UpArrow) {
                    string matchString = searchField;
                    selectedResult = -1;
                    matching.Clear ();

                    OnSearch (matchString);
                    selectedResult = matching.Count > 0 ? 0 : -1;
                }
                GUI.EndGroup ();

                if (searchWantsFocus) {
                    GUIUtility.keyboardControl = controlId;
                    EditorGUIUtility.editingTextField = true;
                    Focus ();
                }
            }
        }

        private void HandleInputEvents () {
            if (Event.current.type == EventType.KeyDown) {
                if (Event.current.keyCode == KeyCode.Escape) {
                    Close ();
                } else if (Event.current.modifiers == settings.OpenShortcut.Modifiers &&
                    Event.current.keyCode == settings.OpenShortcut.Key) {
                    expandWindow = !expandWindow;
                    matching.Clear ();
                    OnSearch = DefaultSearch;
                    Event.current.Use ();
                } else if (Event.current.modifiers == settings.HistoryShortcut.Modifiers &&
                    Event.current.keyCode == settings.HistoryShortcut.Key) {
                    HistoryMode ();
                    Event.current.Use ();
                    // } else if (Event.current.keyCode == KeyCode.Backspace) {
                    //     matchStack.Pop ();
                }
            } else if (Event.current.type == EventType.ValidateCommand && (Event.current.commandName == "Paste" || Event.current.commandName == "Copy")) { // TODO maybe copy copies row info?
                Event.current.Use ();
            } else if (Event.current.type == EventType.ExecuteCommand) {
                if (Event.current.commandName == "Paste") {
                    searchField = EditorGUIUtility.systemCopyBuffer;
                    // matchStack.Clear ();
                    // TODO do new matches populate
                } else if (Event.current.commandName == "Copy") {
                    EditorGUIUtility.systemCopyBuffer = searchField;
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
                }
            }
            if (textEd != null) {
                prevCursorPos = textEd.cursorIndex;
                prevSelectCursorPos = textEd.selectIndex;
            }

            if (Event.current.keyCode == KeyCode.DownArrow || Event.current.keyCode == KeyCode.UpArrow) {
                textEd.MoveTextEnd ();
                if (Event.current.type == EventType.Used) {
                    selectedResult += 2 * (Event.current.keyCode - KeyCode.UpArrow) - 1;
                    CycleHighlightedResult ();
                }
            } else if (Event.current.keyCode == KeyCode.Tab) {
                searchWantsFocus = true;
                if (Event.current.type == EventType.KeyDown) {
                    if (Event.current.modifiers == EventModifiers.None) {
                        selectedResult++;
                    } else if (Event.current.modifiers == EventModifiers.Shift) {
                        selectedResult--;
                    }
                    CycleHighlightedResult ();
                }
                Event.current.Use ();
            } else if (selectedResult != -1 && Event.current.keyCode == KeyCode.Return) {
                ExecuteResult ();
            }
        }

        private void IconButtonGUI () {
            if (GUILayout.Button (m_styles.favoriteIcon, m_styles.iconBtn, GUILayout.Height (18f))) {
                Debug.Log ("Nothing implemented for favorites. Open to suggestions.");
            }
            if (GUILayout.Button (m_styles.historyIcon, m_styles.iconBtn, GUILayout.Height (18f))) {
                HistoryMode ();
            }

            // Maybe open settings/preferences
            // var asm = Assembly.GetAssembly(typeof(EditorWindow));
            // var T=asm.GetType("UnityEditor.PreferencesWindow");
            // var M=T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic|BindingFlags.Static);
            // M.Invoke(null, null);
        }

        private void MatchingScrollGUI () {
            matchingScroll = EditorGUILayout.BeginScrollView (matchingScroll, GUILayout.Width (0.8f * position.width));
            int len = matching.Count;
            len = Mathf.Min (10, len);
            for (int i = 0; i < len; i++) {
                MatchingItem (matching[i], selectedResult == i, i);
            }
            EditorGUILayout.EndScrollView ();
        }

        private void CycleHighlightedResult () {
            selectedResult = Mathf.Clamp (selectedResult, -1, matching.Count - 1);
            // TODO Move the scroll position
            var scrollTo = (selectedResult + 1) * m_styles.iconSize;
            var scrollHeight = position.height - m_styles.searchFieldHeight;
            if (matchingScroll.y > scrollTo || (matchingScroll.y + scrollHeight) < scrollTo) {
                matchingScroll.y = scrollTo - m_styles.iconSize;
            }
        }

        private void MatchingItem (Searchable.ISearchable item, bool selected, int i) {
            if (item == null) {
                // Debug.Log ("NULL"); // TODO figure out why sometimes null, might be b/c parallelized
                return;
            }
            if (selected) {
                EditorGUILayout.BeginHorizontal (m_styles.resultRowSelected);
            } else {
                EditorGUILayout.BeginHorizontal (m_styles.resultRowDefault); // TODO GUIStyle.none doesn't work for some reason??
            }
            GUILayout.Label (item.Icon, GUILayout.Width (m_styles.iconSize), GUILayout.Height (m_styles.iconSize));
            EditorGUILayout.BeginVertical ();
            GUILayout.Label (item.DisplayName, m_styles.resultCommand);
            GUILayout.Label (item.Description, m_styles.resultDescription);
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();

            var labelRect = GUILayoutUtility.GetLastRect ();
            if (labelRect.Contains (Event.current.mousePosition)) {
                if (Event.current.type == EventType.MouseDown) {
                    selectedResult = i;
                    if (Event.current.clickCount == 2) {
                        ExecuteResult ();
                    }
                    Event.current.Use ();
                }

                if (item is Searchable.Asset) { // TODO dont love this being here
                    var asset = item as Searchable.Asset;
                    var mainAsset = asset.GetMainAsset ();
                    if (mainAsset is MonoScript) {
                        var isClass = (mainAsset as MonoScript).GetClass ();
                        if (isClass == null || isClass.IsSubclassOf (typeof (MonoBehaviour))) {
                            return;
                        }
                    }

                    EditorGUIUtility.AddCursorRect (labelRect, MouseCursor.Link);
                    if (Event.current.type == EventType.MouseDrag) {
                        DragAndDrop.PrepareStartDrag ();
                        DragAndDrop.objectReferences = new UnityEngine.Object[] {
                            asset.GetMainAsset ()
                        };
                        DragAndDrop.StartDrag ("QuickFind::Asset Drag");
                        Event.current.Use ();
                    }
                }
            }
        }

        private void ExecuteResult () {
            matching[selectedResult].Execute ();
            QuickFindCache.history.Add (matching[selectedResult]);
            Close ();
        }

        private void DefaultSearch (string matchString) {
            // if (matchStack.Count > 0) {
            //     matching = new List<Searchable.ISearchable> (matchStack.Peek ().Count); // TODO is this worth putting here?
            //     QuickFindUtility.SearchISearchable (matching, matchStack.Peek (), matchString);
            // } else {
            //     matchStack.Push (matching);
            QuickFindUtility.SearchAssets (matching, matchString);
            if (!matchString.Contains (':')) {
                QuickFindUtility.SearchMenuItems (matching, matchString);
                QuickFindUtility.SearchGameObjects (matching, matchString);
            }
            // }
        }

        private void HistoryMode () {
            OnSearch = HistorySearch;
            matching.Clear ();
            HistorySearch (searchField);
            expandWindow = true;
        }

        private void HistorySearch (string matchString) { // TODO parallelize
            Debug.Log (QuickFindCache.history.Count);
            if (string.IsNullOrEmpty (matchString)) {
                matching = new List<Searchable.ISearchable> (QuickFindCache.history);
            } else {
                matching.Capacity = QuickFindCache.history.Count;
                System.Threading.Tasks.Parallel.ForEach (QuickFindCache.history, (item) => {
                    Debug.Log (item.Command);
                    if (item.Command.Contains (matchString, StringComparison.OrdinalIgnoreCase)) {
                        matching.Add (item);
                    }
                });
            }
        }
    }
}