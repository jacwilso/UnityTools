using System.Collections.Generic;
using UnityEngine;

namespace TaskManager.Data {
    // [CreateAssetMenu (fileName = "Board", menuName = "TaskManager/Board", order = 0)]
    public class Board : ScriptableObject {
        public string title;
        public List<Card> cards = new List<Card> ();
    }
}