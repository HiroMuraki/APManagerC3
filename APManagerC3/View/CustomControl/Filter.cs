using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace APManagerC3.View {
    public class Filter : Control {
        public static readonly DependencyProperty FilterCategoryProperty =
            DependencyProperty.Register(nameof(FilterCategory), typeof(string), typeof(Filter), new PropertyMetadata(""));
        public static readonly DependencyProperty FilterStatusProperty =
            DependencyProperty.Register(nameof(FilterStatus), typeof(Status), typeof(Filter), new PropertyMetadata(Status.Disable));
        public static readonly DependencyProperty FilterIdentifierProperty =
            DependencyProperty.Register(nameof(FilterIdentifier), typeof(Brush), typeof(Filter), new PropertyMetadata(null));

        public event EventHandler<DataDragDropEventArgs> DataDragDrop;

        public string FilterCategory {
            get { return (string)GetValue(FilterCategoryProperty); }
            set { SetValue(FilterCategoryProperty, value); }
        }
        public Status FilterStatus {
            get { return (Status)GetValue(FilterStatusProperty); }
            set { SetValue(FilterStatusProperty, value); }
        }
        public Brush FilterIdentifier {
            get { return (Brush)GetValue(FilterIdentifierProperty); }
            set { SetValue(FilterIdentifierProperty, value); }
        }

        static Filter() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Filter), new FrameworkPropertyMetadata(typeof(Filter)));
        }
        public Filter() {

        }

        public override void OnApplyTemplate() {
            DragOver += Filter_DragOver;
            DragLeave += Filter_DragLeave;
            Drop += Filter_Drop;
        }

        private void Filter_DragOver(object sender, DragEventArgs e) {
            var formats = e.Data.GetFormats();
            if (e.Data.IsType(typeof(ViewModel.Filter))) {
                var objData = e.Data.GetData(typeof(ViewModel.Filter)) as ViewModel.Filter;
                if (objData == null) {
                    return;
                }
                var relatePos = e.GetPosition(this);
                if (relatePos.Y <= ActualHeight / 2) {
                    ShowTipBorder(Direction.Up);
                }
                else {
                    ShowTipBorder(Direction.Down);
                }
            }
            else if (e.Data.IsType(typeof(ViewModel.Container))) {
                var objData = e.Data.GetData(typeof(ViewModel.Container)) as ViewModel.Container;
                if (objData == null) {
                    return;
                }
                ShowTipBorder(Direction.Up | Direction.Down | Direction.Left | Direction.Right);
            }

        }
        private void Filter_DragLeave(object sender, DragEventArgs e) {
            ResetTipBorder();
        }
        private void Filter_Drop(object sender, DragEventArgs e) {
            var relatePos = e.GetPosition(this);
            if (relatePos.Y <= ActualHeight / 2) {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Up, e.Data));
            }
            else {
                DataDragDrop?.Invoke(this, new DataDragDropEventArgs(Direction.Down, e.Data));
            }
            ResetTipBorder();
        }
        private void ShowTipBorder(Direction direction) {
            var tipBorder = Template.FindName("PART_TipBorder", this) as Border;
            Thickness thickness = new Thickness();
            if ((direction & Direction.Up) == Direction.Up) {
                thickness.Top = 5;
            }
            if ((direction & Direction.Down) == Direction.Down) {
                thickness.Bottom = 5;
            }
            if ((direction & Direction.Left) == Direction.Left) {
                thickness.Left = 5;
            }
            if ((direction & Direction.Right) == Direction.Right) {
                thickness.Right = 5;
            }
            tipBorder.BorderThickness = thickness;
        }
        private void ResetTipBorder() {
            var tipBorder = Template.FindName("PART_TipBorder", this) as Border;
            tipBorder.BorderThickness = new Thickness(0);
        }
    }
}
