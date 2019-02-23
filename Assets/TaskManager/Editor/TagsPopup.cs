using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TaskManager {
    public class TagsPopup : PopupWindowContent {
        private List<string> tags;

        public override void OnOpen () {

        }

        public override void OnGUI (Rect rect) {
            foreach (var tag in tags) { }
        }
    }
}