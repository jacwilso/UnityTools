using System.Collections.Generic;

namespace TaskManager.Data {
    [System.Serializable]
    public class Card {
        public string title = string.Empty;
        public List<Task> tasks = new List<Task> ();
    }
}