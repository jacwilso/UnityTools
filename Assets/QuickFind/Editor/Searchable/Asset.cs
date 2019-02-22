using UnityEditor;

namespace QuickFind.Editor.Searchable {
    public class Asset : ISearchable {
        private string path;
        private string type;

        public string Command => path;

        public string Description => type;

        public string IconPath => path;

        public Asset (string p) {
            path = p;
            type = AssetDatabase.GetMainAssetTypeAtPath (path).Name;
        }

        public void Execute () {
            var assetType = AssetDatabase.GetMainAssetTypeAtPath (path);
            UnityEngine.Debug.Log (type);
            var mainAsset = AssetDatabase.LoadMainAssetAtPath (path);
            EditorGUIUtility.PingObject (mainAsset);
            // UnityEngine.Event.current.Use ();
        }
    }
}