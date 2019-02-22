using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace QuickFind.Editor {
    public static class QuickFindUtility {

        public static Func<string, bool> Filter (char filterType, string filter) {
            switch (filterType) {
                case 't':
                    return (string x) => {
                        return AssetDatabase.GetMainAssetTypeAtPath (x).Name.Equals (filter, StringComparison.OrdinalIgnoreCase); // TODO Equals or StartsWith (settings option?)
                    };
                case 'l':
                    return (string x) => { return AssetDatabase.GetLabels (AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (x)).Contains (filter); }; // TOOD ignore case?
            }
            return (string x) => { return true; };
        }
    }
}