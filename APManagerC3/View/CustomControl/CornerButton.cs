using System.Windows;
using System.Windows.Controls;

namespace APManagerC3.View {
    public class CornerButton : Button {
        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CornerButton), new PropertyMetadata(new CornerRadius(0)));

        static CornerButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CornerButton), new FrameworkPropertyMetadata(typeof(CornerButton)));
        }
    }
}
