using System.Windows.Input;

namespace APManagerC3 {
    public static class ContainerCommands {
        public static RoutedCommand New { get; } = new RoutedCommand();
        public static RoutedCommand Duplicate { get; } = new RoutedCommand();
        public static RoutedCommand Remove { get; } = new RoutedCommand();
    }
}
