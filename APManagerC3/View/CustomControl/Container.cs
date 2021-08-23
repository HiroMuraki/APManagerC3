using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace APManagerC3.View {
    public class Container : Control {
        public static readonly DependencyProperty ContainerIdentifierProperty =
            DependencyProperty.Register(nameof(ContainerIdentifier), typeof(Brush), typeof(Container), new PropertyMetadata(null));
        public static readonly DependencyProperty ContainerTitleProperty =
            DependencyProperty.Register(nameof(ContainerTitle), typeof(string), typeof(Container), new PropertyMetadata(""));
        public static readonly DependencyProperty ContainerStatusProperty =
            DependencyProperty.Register(nameof(ContainerStatus), typeof(Status), typeof(Container), new PropertyMetadata(Status.Disable));

        public event EventHandler<DataDragDropEventArgs> DataDragDrop;

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

        public override void OnApplyTemplate() {
            DragEnter += Container_DragEnter;
            DragLeave += Container_DragLeave;
            Drop += Container_Drop;
        }

        private void Container_DragEnter(object sender, DragEventArgs e) {
            var relatePos = e.GetPosition(this);
            if (relatePos.Y <= ActualHeight / 2) {
                ShowTipLine(Direction.Up);
            }
            else {
                ShowTipLine(Direction.Down);
            }
        }
        private void Container_Drop(object sender, DragEventArgs e) {
            var relatePos = e.GetPosition(this);
            if (relatePos.Y <= ActualHeight / 2) {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Up, e.Data));
            }
            else {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Down, e.Data));
            }
            ResetTipLine();
        }
        private void Container_DragLeave(object sender, DragEventArgs e) {
            ResetTipLine();
        }

        private void ShowTipLine(Direction direction) {
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
        private void ResetTipLine() {
            var tipBorder = Template.FindName("PART_TipBorder", this) as Border;
            tipBorder.BorderThickness = new Thickness(0);
        }

        static Container() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Container), new FrameworkPropertyMetadata(typeof(Container)));
        }
    }
}
