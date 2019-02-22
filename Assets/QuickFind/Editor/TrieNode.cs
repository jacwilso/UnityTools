using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace QuickFind.Editor {
    public class TrieNode : IEnumerable<TrieNode> {
        public const int ALPHABET_SIZE = 26;
        private const char START_CHAR = 'a';

        private TrieNode[] children = new TrieNode[ALPHABET_SIZE];
        private bool isEnd = false;
        // private List<Object> userData;

        public IEnumerator<TrieNode> GetEnumerator () {
            foreach (var node in children) {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        public void Add (string key) {
            var node = this;

            int len = key.Length;
            for (int i = 0; i < len; i++) {
                int index = key[i] - START_CHAR;
                if (node.children[index] == null) {
                    node.children[index] = new TrieNode ();
                }
                node = node.children[index];
            }

            node.isEnd = true;
        }

        public void RemoveUnique (string key) {
            var node = this;
            var stack = new Stack<TrieNode> ();

            int len = key.Length;
            for (int i = 0; i < len; i++) {
                int index = key[i] - START_CHAR;
                node = node.children[index];
                stack.Push (node);
            }

            node = stack.Pop ();
            for (int i = len - 1; i >= 0; i++) {
                node = stack.Pop ();
                int index = key[i] - START_CHAR;
                node.children[index] = null;
                if (node.isEnd) {
                    break;
                }
            }
        }

        public bool ContainsKey (string key) {
            var node = this;

            int len = key.Length;
            for (int i = 0; i < len; i++) {
                int index = key[i] - START_CHAR;
                if (node.children[index] == null) {
                    return false;
                }
                node = node.children[index];
            }

            return node != null && node.isEnd;
        }

        public List<string> FindKeysPrefixed (string key) {
            var node = this;

            int len = key.Length;
            for (int i = 0; i < len; i++) {
                int index = key[i] - START_CHAR;
                if (node.children[index] == null) {
                    return null;
                }
                node = node.children[index];
            }

            var keys = new List<string> ();
            var keyStr = new StringBuilder (key);
            node.GetKeys (keys, keyStr);
            return keys;
        }

        // TODO test
        public List<string> FindAllContaining (string key) {
            var keys = new List<string> ();
            var keyStr = new StringBuilder (key);
            FindAllContaining (keys, key, keyStr);
            return keys;
        }

        private void FindAllContaining (List<string> keys, string key, StringBuilder keyStr) {
            if (isEnd) {
                return;
            }

            var node = this;
            bool containsKey = true;
            int len = key.Length;
            for (int i = 0; i < len; i++) {
                int index = key[i] - START_CHAR;
                if (node.children[index] == null) {
                    containsKey = false;
                    break;
                }
                node = node.children[index];
            }

            if (containsKey) {
                Debug.Log (keyStr);
                int keyStrLen = keyStr.Length;
                keyStr.Append (key);
                node.GetKeys (keys, keyStr);
                keyStr.Remove (keyStrLen, keyStr.Length - keyStrLen);
            }

            node = this;
            for (int i = 0; i < ALPHABET_SIZE; i++) {
                if (node.children[i] != null) {
                    node.children[i].FindAllContaining (keys, key, keyStr);
                }
            }
        }

        public List<string> GetKeys () {
            var keys = new List<string> ();
            var keyStr = new StringBuilder ();

            GetKeys (keys, keyStr);
            return keys;
        }

        private void GetKeys (List<string> keys, StringBuilder keyStr) {
            if (isEnd) {
                keys.Add (keyStr.ToString ());
            }

            int strLen = keyStr.Length;
            for (int i = 0; i < ALPHABET_SIZE; i++) {
                if (children[i] != null) {
                    var c = (char) (START_CHAR + i);
                    keyStr.Append ((char) (START_CHAR + i));
                    children[i].GetKeys (keys, keyStr);
                    keyStr.Remove (strLen, keyStr.Length - strLen);
                }
            }
        }
    }
}