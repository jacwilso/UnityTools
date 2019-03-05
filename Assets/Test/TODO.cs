using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TODO", menuName = "QuickFind/TODO", order = 0)]
public class TODO : ScriptableObject {
    [TextArea (40, 100)]
    public string todo;

    public List<string> m_todo;
    public List<string> m_done;

    [UnityEditor.MenuItem ("Test/TEST")]
    private static void TestMenuItem () {

    }
}