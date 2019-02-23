using UnityEditor;
using UnityEngine;

namespace QuickFind.Editor.Searchable {
    public class Asset : ISearchable {
        private string path;
        private System.Type type;
        // private Object mainAsset;

        public string Command => path;

        private string displayName;
        public string DisplayName => displayName;

        public string Description => type.Name;

        private Texture icon;
        public Texture Icon {
            get {
                if (icon == null) {
                    // icon = AssetPreview.GetMiniTypeThumbnail (type);

                    // mainAsset = AssetDatabase.LoadMainAssetAtPath (path);
                    // icon = AssetPreview.GetMiniThumbnail (mainAsset);
                    icon = AssetDatabase.GetCachedIcon (path);

                }
                return icon;
            }
        }

        public Asset (string p) {
            path = p;
            displayName = p.Substring (p.LastIndexOf ('/') + 1);
            type = AssetDatabase.GetMainAssetTypeAtPath (path);
        }

        public void Execute () {
            var mainAsset = AssetDatabase.LoadMainAssetAtPath (path);
            EditorGUIUtility.PingObject (mainAsset);
            AssetDatabase.OpenAsset (mainAsset);
        }
    }
}