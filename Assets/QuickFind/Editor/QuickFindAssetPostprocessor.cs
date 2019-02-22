using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickFind.Editor {
    /*
    class QuickFindAssetPostprocessor : AssetPostprocessor {
        private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssetsTo, string[] movedAssetsFrom) {
            if (QuickFindCache.AssetPaths == null) {
                return;
            }
            // foreach (string str in importedAssets) {
            //     // Debug.Log ("Reimported Asset: " + str);

            //     if (!QuickFindEditor.AssetPaths.Keys.Any (x => x.Equals (str))) {
            //         QuickFindEditor.AssetPaths.Add (str, AssetDatabase.LoadMainAssetAtPath (str));
            //     }
            // }
            foreach (string str in deletedAssets) {
                // Debug.Log ("Deleted Asset: " + str);
                QuickFindCache.LoadedAssets.Remove (str);
            }

            for (int i = 0; i < movedAssetsTo.Length; i++) {
                // Debug.Log ("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
                Object movedObject;
                QuickFindCache.LoadedAssets.TryGetValue (movedAssetsFrom[i], out movedObject);
                if (movedObject != null) {
                    QuickFindCache.LoadedAssets.Add (movedAssetsTo[i], movedObject);
                    QuickFindCache.LoadedAssets.Remove (movedAssetsFrom[i]);
                }
            }
        }
    }
    */
}