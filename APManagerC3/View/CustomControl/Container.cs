using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace APManagerC3.View {
    public class Container : Control {
        public static readonly DependencyProperty ContainerIdentifierProperty =
            DependencyProperty.Register(nameof(ContainerIdentifier), typeof(Brush), typeof(Container), new PropertyMetadata(null));
        public static readonly DependencyProperty ContainerTitleProperty =
            DependencyProperty.Register(nameof(ContainerTitle), typeof(string), typeof(Container), new PropertyMetadata(""));
        public static readonly DependencyProperty ContainerStatusProperty =
            DependencyProperty.Register(nameof(ContainerStatus), typeof(Status), typeof(Container), new PropertyMetadata(Status.Disable));

        public string ContainerTitle {
            get { return (string)GetValue(ContainerTitleProperty); }
            set { SetValue(ContainerTitleProperty, value); }
        }
        public Status ContainerStatus {
            get { return (Status)GetValue(ContainerStatusProperty); }
            set { SetValue(ContainerStatusProperty, value); }
        }
        public Brush ContainerIdentifier {
            get { return (Brush)GetValue(ContainerIdentifierProperty); }
            set { SetValue(ContainerIdentifierProperty, value); }
        }

        static Container() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Container), new FrameworkPropertyMetadata(typeof(Container)));
        }
    }
}
