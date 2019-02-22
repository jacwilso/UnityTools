using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
// using UnityEditor.Compilation;
using UnityEngine;

namespace QuickFind.Editor {
    public static class QuickFindCache {
        // struct MainAsset {
        //     public Type type;
        //     public UnityEngine.Object obj;
        // }

        public static IEnumerable<string> AssetPaths { get; private set; }
        // private static Dictionary<string, MainAsset> LoadedAssets { get; set; }

        public static void Awake () {
            AssetPaths = AssetDatabase.GetAllAssetPaths ();
            if (!QuickFind.Settings.QuickFindSettings.GetOrCreateSettings ().IncludeProjectFiles)
                AssetPaths = AssetPaths.Where (x => x.StartsWith ("Assets"));

            // TODO Remove
            // foreach (var assem in System.AppDomain.CurrentDomain.GetAssemblies ()) {
            //     // var dic = item.GetTypes ()
            //     //     .SelectMany (x => x.GetMethods (BindingFlags.Static | BindingFlags.NonPublic))
            //     //     .Where (y => y.GetCustomAttributes ().OfType<MenuItem> ().Any ()).
            //     var dic = assem.GetTypes ()
            //         .SelectMany (x => x.GetMethods (BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            //         .Where (y => {
            //             var item = y.GetCustomAttribute<MenuItem> ();
            //             return item != null && !item.validate;
            //         })
            //         .ToDictionary (z => z.GetCustomAttribute<MenuItem> ().menuItem);
            //     // .Where (y => y.GetCustomAttributes (typeof (MenuItem)).Any ())
            //     // .Where (y => y.GetCustomAttributes ().OfType<MenuItem> ().Where (x => !x.validate).Any ())
            //     // .Select (z => {
            //     //     var menuItem = z.GetCustomAttribute (typeof (MenuItem)) as MenuItem;
            //     //     return menuItem.menuItem;
            //     // })
            //     // .ToDictionary (z => z.Name);
            //     // .ToList ();
            //     int cnt = dic.Count ();
            //     if (cnt == 0) continue;
            //     Debug.Log ($"[{cnt}] " + assem);
            //     foreach (var kv in dic) {
            //         Debug.Log (kv);
            //     }
            //     // break;
            // }

            // Assembly.GetAssembly(typeof(MenuItem))
            // foreach (var a in methods) {
            //     UnityEngine.Debug.Log (a.Key);
            // }
        }

        // public static UnityEngine.Object Add (string asset) {
        //     var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (asset);
        //     LoadedAssets.Add (asset, new MainAsset () { obj = obj });
        //     return obj;
        // }

        // public static void Remove (string asset) {
        //     LoadedAssets.Remove (asset);
        // }

        private static readonly Dictionary<string, MethodInfo> menuItems =
            System.AppDomain.CurrentDomain.GetAssemblies ()
            .SelectMany (x => x.GetTypes ())
            .SelectMany (x => x.GetMethods (BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Where (y => {
                var item = y.GetCustomAttribute<MenuItem> ();
                return item != null && !item.validate;
            })
            .ToDictionary (z => z.GetCustomAttribute<MenuItem> ().menuItem);

        public static Dictionary<string, MethodInfo> MenuItems { // TODO possibly readonly
            get { return menuItems; }
        }
    }
}