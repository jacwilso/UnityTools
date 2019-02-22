namespace QuickFind {
    public interface ICommand {
        string command { get; }
        void Execute (string command);
    }
}