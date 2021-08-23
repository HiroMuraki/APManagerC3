using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace APManagerC3.View {
    public class Record : Control {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Record), new PropertyMetadata(""));
        public static readonly DependencyProperty InformationProperty =
            DependencyProperty.Register(nameof(Information), typeof(string), typeof(Record), new PropertyMetadata(""));

        public event RoutedEventHandler Remove;
        public event MouseButtonEventHandler DragHandlerHold;
        public event EventHandler<DataDragDropEventArgs> DataDragDrop;

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
            dragHandlerButton.DragOver += DragHandlerButton_DragOver; ;
            dragHandlerButton.DragLeave += DragHandlerButton_DragLeave;
            dragHandlerButton.Drop += DragHandlerButton_Drop;
        }

        private void DragHandlerButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragHandlerHold?.Invoke(this, e);
        }
        private void DragHandlerButton_DragOver(object sender, DragEventArgs e) {
            var relatePos = e.GetPosition(this);
            if (relatePos.Y <= ActualHeight / 2) {
                ShowTipBorder(Direction.Up);
            }
            else {
                ShowTipBorder(Direction.Down);
            }
        }
        private void DragHandlerButton_Drop(object sender, DragEventArgs e) {
            var relatePos = e.GetPosition(this);
            if (relatePos.Y <= ActualHeight / 2) {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Up, e.Data));
            }
            else {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Down, e.Data));
            }
            ResetTipBorder();
        }
        private void DragHandlerButton_DragLeave(object sender, DragEventArgs e) {
            ResetTipBorder();
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            Remove?.Invoke(this, e);
        }

        private void ResetTipBorder() {
            var tipBorder = Template.FindName("PART_TipBorder", this) as Border;
            tipBorder.BorderThickness = new Thickness(0);
        }
        private void ShowTipBorder(Direction direction) {
            var tipBorder = Template.FindName("PART_TipBorder", this) as Border;
            Thickness thickness = new Thickness(0);
            switch (direction) {
                case Direction.Up:
                    thickness.Top = 5;
                    break;
                case Direction.Down:
                    thickness.Bottom = 5;
                    break;
                case Direction.Left:
                    thickness.Left = 5;
                    break;
                case Direction.Right:
                    thickness.Right = 5;
                    break;
                default:
                    break;
            }
            tipBorder.BorderThickness = thickness;
        }
    }
}
