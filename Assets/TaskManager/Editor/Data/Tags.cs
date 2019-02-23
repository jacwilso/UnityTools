using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskManager.Data {
    [System.Serializable]
    public class Tags : ISerializationCallbackReceiver {
        public HashSet<string> tags;

        private List<string> serializedTags;

        public void OnAfterDeserialize () {
            tags = new HashSet<string> (serializedTags);
        }

        public void OnBeforeSerialize () {
            serializedTags = tags.ToList ();
        }
    }
}