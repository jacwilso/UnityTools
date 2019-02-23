using System.Collections.Generic;

namespace TaskManager.Data {
    [System.Serializable]
    public class Task {
        public string title = string.Empty;
        public string description = string.Empty;

        // modifiers/labels

        public UnityEngine.Object assets;
        public List<string> tags = new List<string> ();

        public List<SubTask> subTasks = new List<SubTask> (); // TODO why not just task
    }
}