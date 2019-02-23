using System;

namespace QuickFind.Editor.Searchable {
    public interface ISearchable {
        string Command { get; }
        string DisplayName { get; }
        string Description { get; }
        UnityEngine.Texture Icon { get; }

        void Execute ();
    }
}