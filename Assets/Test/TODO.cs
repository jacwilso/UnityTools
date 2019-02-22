using UnityEngine;

[CreateAssetMenu (fileName = "TODO", menuName = "QuickFind/TODO", order = 0)]
public class TODO : ScriptableObject {
    [TextArea (40, 100)]
    public string todo;
}