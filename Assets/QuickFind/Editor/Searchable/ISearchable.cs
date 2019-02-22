using System;

namespace QuickFind.Editor.Searchable {
    public interface ISearchable {
        string Command { get; }
        string Description { get; }
        string IconPath { get; }

        void Execute ();
    }
}