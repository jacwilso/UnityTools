using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
// using UnityEditor.Compilation;
using UnityEngine;

namespace QuickFind.Editor {
    public static class QuickFindCache {

        private static bool refreshAssets = true;
        private static string[] assetPaths;
        public static string[] AssetPaths {
            get {
                if (refreshAssets) {
                    refreshAssets = false;
                    assetPaths = AssetDatabase.GetAllAssetPaths ();
                    if (!QuickFind.Settings.QuickFindSettings.GetOrCreateSettings ().IncludeProjectFiles) {
                        assetPaths = assetPaths.Where (x => x.StartsWith ("Assets")).ToArray ();
                    }
                }
                return assetPaths;
            }
        }

        private static bool refreshGameObjects = true;
        private static GameObject[] sceneGameObjects;
        public static GameObject[] SceneGameObjects {
            get {
                if (refreshGameObjects) {
                    refreshGameObjects = false;
                    sceneGameObjects = GameObject.FindObjectsOfType<GameObject> (); // TODO think about processing them here
                }
                return sceneGameObjects;
            }
        }

        private static readonly Searchable.MenuItemCommand[] menuItems = System.AppDomain.CurrentDomain.GetAssemblies ()
            .SelectMany (x => x.GetTypes ())
            .SelectMany (x => x.GetMethods (BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Where (x => {
                var item = x.GetCustomAttribute<MenuItem> ();
                return item != null && !item.validate;
            })
            .Select (x => new Searchable.MenuItemCommand (x.GetCustomAttribute<MenuItem> ().menuItem, x))
            .ToArray ();
        public static Searchable.MenuItemCommand[] MenuItems {
            get { return menuItems; }
        }

        public static List<Searchable.ISearchable> history = new List<Searchable.ISearchable> ();

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad () { // TODO Should I call and cache, or wait for open
            EditorApplication.hierarchyChanged += OnHierarchyChange;
        }

        private static void OnHierarchyChange () {
            refreshGameObjects = true;
        }

        private class QuickFindAssetPostprocessor : AssetPostprocessor {
            private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssetsTo, string[] movedAssetsFrom) {
                refreshAssets = true;
            }
        }
    }
}