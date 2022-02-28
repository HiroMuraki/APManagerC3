using System.Windows.Input;

namespace APManagerC3 {
    public static class RecordCommands {
        public static RoutedCommand New { get; } = new RoutedCommand();
        public static RoutedCommand Remove { get; } = new RoutedCommand();
    }
}
