using System.Windows.Input;

namespace APManagerC3 {
    public static class WindowCommands {
        public static RoutedCommand Minimum { get; } = new RoutedCommand();
        public static RoutedCommand Maximum { get; } = new RoutedCommand();
        public static RoutedCommand Close { get; } = new RoutedCommand();
    }
}
