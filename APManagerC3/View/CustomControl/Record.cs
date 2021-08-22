using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace APManagerC3.View {
    public class Record : Control {
        public static readonly RoutedEvent RemoveEvent =
            EventManager.RegisterRoutedEvent(nameof(Remove), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Record));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Record), new PropertyMetadata(""));
        public static readonly DependencyProperty InformationProperty =
            DependencyProperty.Register(nameof(Information), typeof(string), typeof(Record), new PropertyMetadata(""));

        public event RoutedEventHandler Remove {
            add {
                AddHandler(RemoveEvent, value);
            }
            remove {
                RemoveHandler(RemoveEvent, value);
            }
        }
        public event MouseButtonEventHandler DragHandlerHold;

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public string Information {
            get { return (string)GetValue(InformationProperty); }
            set { SetValue(InformationProperty, value); }
        }

        static Record() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Record), new FrameworkPropertyMetadata(typeof(Record)));
        }
        public override void OnApplyTemplate() {
            var removeButton = Template.FindName("PART_RemoveButton", this) as Button;
            var dragHandlerButton = Template.FindName("PART_DragHandler", this) as Button;
            removeButton.Click += RemoveButton_Click;
            dragHandlerButton.PreviewMouseLeftButtonDown += DragHandlerButton_PreviewMouseLeftButtonDown;
        }

        private void DragHandlerButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragHandlerHold?.Invoke(this, e);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            RoutedEventArgs arg = new RoutedEventArgs(RemoveEvent, this);
            RaiseEvent(arg);
        }
    }
}
