using System.Reflection;

namespace QuickFind.Editor.Searchable {
    public class MenuItemCommand : ISearchable {
        private string command;
        public string Command => command;

        public string DisplayName => command;

        private string description = string.Empty;
        public string Description => description;

        private UnityEngine.Texture icon;
        public UnityEngine.Texture Icon {
            get {
                if (icon == null) {
                    icon = UnityEditor.EditorGUIUtility.IconContent ("UnityLogo.png").image;
                }
                return icon;
            }
        }

        private MethodInfo methodInfo;

        public MenuItemCommand (string c, MethodInfo m) {
            int from = c.LastIndexOf ('/') + 1;
            int to = c.IndexOfAny (new char[] { '#', '%', '&' });
            if (to == -1) {
                command = c.Substring (from);
                // description = "No shortcut";
            } else {
                command = c.Substring (from, to - from - 1);
                // description = "Shortcut: " + c.Substring (to);
            }
            description = c;
            methodInfo = m;
        }

        public void Execute () {
            try {
                // if (methodInfo.ReturnType == typeof (void)) {
                if (methodInfo.GetParameters ().Length > 0) {
                    // MenuCommand
                    // TODO Selection.activeContext > filter if not available
                    methodInfo.Invoke (null, new object[] { new UnityEditor.MenuCommand (null, 0) });
                } else {
                    methodInfo.Invoke (null, null);
                }
                // } else {
                //     //ASSET STORE
                // }
            } catch (System.Exception e) {
                UnityEngine.Debug.LogWarning ("Error");
                UnityEngine.Debug.LogWarning (e.Message);
            }
        }
    }
}