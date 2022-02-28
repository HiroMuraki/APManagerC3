using System.Windows.Input;

namespace APManagerC3 {
    public static class APMCommands {
        public static RoutedCommand Save { get; } = new RoutedCommand();
        public static RoutedCommand Load { get; } = new RoutedCommand();
    }
}
