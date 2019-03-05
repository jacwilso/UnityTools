using UnityEditor;
using UnityEngine;

namespace QuickFind.Editor.Searchable {
    public class Asset : ISearchable {
        private string guid;

        private string path;
        private string Path {
            get {
                if (string.IsNullOrEmpty (path)) {
                    path = AssetDatabase.GUIDToAssetPath (guid);
                }
                return path;
            }
        }

        private System.Type assetType;
        public System.Type AssetType {
            get {
                if (assetType == null) {
                    assetType = AssetDatabase.GetMainAssetTypeAtPath (Path);
                }
                return assetType;
            }
        }
        // private Object mainAsset;

        public string Command => Path;

        private string displayName;
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty (displayName)) {
                    displayName = System.IO.Path.GetFileNameWithoutExtension (Path);
                }
                return displayName;
            }
        }

        public string Description {
            get {
                return $"{Path} ({AssetType.Name})";
            }
        }

        private Texture icon;
        public Texture Icon {
            get {
                if (icon == null) {
                    icon = AssetDatabase.GetCachedIcon (Path);
                }
                return icon;
            }
        }

        public Asset (string g) {
            guid = g;
        }

        public void Execute () {
            var mainAsset = AssetDatabase.LoadMainAssetAtPath (Path);
            AssetDatabase.OpenAsset (mainAsset);
            EditorGUIUtility.PingObject (mainAsset);
        }

        public Object GetMainAsset () {
            return AssetDatabase.LoadMainAssetAtPath (Path);
        }
    }
}