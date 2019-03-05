using UnityEditor;

namespace QuickFind.Editor.Searchable {
    public class GameObject : ISearchable {
        private UnityEngine.GameObject gameObject;

        public string Command => gameObject.name;

        public string DisplayName => gameObject.name;

        public string Description {
            get {
                var scene = gameObject.scene.name;
                return "Scene: " + (string.IsNullOrEmpty (scene) ? "Untitled" : scene);
            }
        }

        private UnityEngine.Texture icon;
        public UnityEngine.Texture Icon {
            get {
                if (icon == null) {
                    icon = EditorGUIUtility.ObjectContent (gameObject, typeof (UnityEngine.GameObject)).image;
                }
                return icon;
            }
        }

        public GameObject (UnityEngine.GameObject go) {
            gameObject = go;
        }

        public void Execute () {
            EditorGUIUtility.PingObject (gameObject);
        }
    }
}