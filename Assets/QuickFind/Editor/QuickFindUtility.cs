using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickFind.Editor {
    public static class QuickFindUtility {

        // private static readonly string[] TypeDisplayNames = {
        //     "AnimationClip",
        //     "AudioClip",
        //     "AudioMixer",
        //     "ComputeShader",
        //     "Font",
        //     "GUISkin",
        //     "Material",
        //     "Mesh",
        //     "Model",
        //     "PhysicMaterial",
        //     "Prefab",
        //     "Scene",
        //     "Script",
        //     "Shader",
        //     "Sprite",
        //     "Texture",
        //     "VideoClip",

        //     // "Texture2D",
        //     // "RenderTexture",
        //     // "Cubemap",
        //     // "MovieTexture",
        // };
        // private static string DisplayNameToType (string filter) { // TODO I really don't like this, and it doesn't really work
        //     switch (filter.ToLower ()) {
        //         case "audiomixer":
        //             return "AudioMixerComponent";
        //         case "prefab":
        //             return "GameObject";
        //         case "scene":
        //             return "SceneAsset";
        //         case "script":
        //             return "MonoScript"; // TODO this includes scriptableobjects which is ok
        //             // TODO can't search scriptable objects right now :(
        //         case "sprite": // TODO this is wrong!!
        //         case "texture":
        //             return "Texture2D"; // TODO this could also be RenderTexture
        //     }
        //     return filter;
        // }

        // public static Func<string, bool> Filter (char filterType, string filter) {
        //     switch (filterType) {
        //         case 't':
        //             return (x) => {
        //                 var typeName = AssetDatabase.GetMainAssetTypeAtPath (x).Name; // TODO this isn't sufficient
        //                 filter = DisplayNameToType (filter);
        //                 return typeName.Equals (filter, StringComparison.OrdinalIgnoreCase); // TODO Equals or StartsWith (settings option?)
        //             };
        //         case 'l':
        //             return (x) => { return AssetDatabase.GetLabels (AssetDatabase.LoadMainAssetAtPath (x)).Contains (filter); }; // TOOD ignore case?
        //     }
        //     return (string x) => { return true; };
        // }

        // public static List<Func<string, bool>> GetSearchFilters (ref string matchString) {
        //     int colonIndex = matchString.IndexOf (':');
        //     int len = matchString.Length - 1;
        //     List<Func<string, bool>> filters = new List<Func<string, bool>> ();
        //     while (colonIndex > 0 && colonIndex < len) {
        //         char filterType = matchString[colonIndex - 1];
        //         var filterEnd = matchString.IndexOf (' ', colonIndex);
        //         string filter;
        //         if (filterEnd == -1) {
        //             filter = matchString.Substring (colonIndex + 1);
        //             matchString = matchString.Remove (colonIndex - 1, matchString.Length - (colonIndex - 1));
        //         } else {
        //             filter = matchString.Substring (colonIndex + 1, filterEnd - (colonIndex + 1));
        //             matchString = matchString.Remove (colonIndex - 1, filterEnd + 1);
        //         }
        //         if (string.IsNullOrEmpty (filter)) {
        //             continue;
        //         }
        //         filters.Add (QuickFindUtility.Filter (filterType, filter));

        //         colonIndex = matchString.IndexOf (':');
        //     }
        //     return filters;
        // }

        public static void SearchAssets (List<Searchable.ISearchable> matching, string matchString) {
            var assetGUIDs = AssetDatabase.FindAssets (matchString);
            System.Threading.Tasks.Parallel.ForEach (assetGUIDs, (guid) => {
                matching.Add (new Searchable.Asset (guid));
            });
        }

        public static void SearchMenuItems (List<Searchable.ISearchable> matching, string matchString) {
            System.Threading.Tasks.Parallel.ForEach (QuickFindCache.MenuItems, (item) => {
                if (item.Command.Contains (matchString, StringComparison.OrdinalIgnoreCase)) {
                    matching.Add (item);
                }
            });
        }

        public static void SearchGameObjects (List<Searchable.ISearchable> matching, string matchString) {
            foreach (var go in QuickFindCache.SceneGameObjects) {
                if (go.name.Contains (matchString, StringComparison.OrdinalIgnoreCase)) {
                    matching.Add (new Searchable.GameObject (go));
                }
            }
        }

        public static void SearchISearchable (List<Searchable.ISearchable> matching, List<Searchable.ISearchable> searchList, string matchString) {
            System.Threading.Tasks.Parallel.ForEach (searchList, (searchable) => {
                if (searchable.Command.Contains (matchString)) {
                    matching.Add (searchable);
                }
            });
        }
    }
}