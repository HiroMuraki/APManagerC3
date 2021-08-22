using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace APManagerC3.View {
    public class Filter : Control {
        private static readonly DoubleAnimation _dragTipFullAnimation = new DoubleAnimation() {
            AccelerationRatio = 0.2,
            DecelerationRatio = 0.2,
            Duration = TimeSpan.FromMilliseconds(50)
        };
        public static readonly DependencyProperty FilterCategoryProperty =
            DependencyProperty.Register(nameof(FilterCategory), typeof(string), typeof(Filter), new PropertyMetadata(""));
        public static readonly DependencyProperty FilterStatusProperty =
            DependencyProperty.Register(nameof(FilterStatus), typeof(Status), typeof(Filter), new PropertyMetadata(Status.Disable));
        public static readonly DependencyProperty FilterIdentifierProperty =
            DependencyProperty.Register(nameof(FilterIdentifier), typeof(Brush), typeof(Filter), new PropertyMetadata(null));

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
            DragEnter += Filter_DragEnter;
            DragLeave += Filter_DragLeave;
            Drop += Filter_Drop;
        }

        private void Filter_DragEnter(object sender, DragEventArgs e) {
            var objData = e.Data.GetData(typeof(ViewModel.Container)) as ViewModel.Container;
            if (objData != null) {
                var highlight = Template.FindName("DragTipFull", this) as FrameworkElement;
                _dragTipFullAnimation.To = 1;
                highlight.BeginAnimation(OpacityProperty, _dragTipFullAnimation);
            }
        }
        private void Filter_DragLeave(object sender, DragEventArgs e) {
            var highlight = Template.FindName("DragTipFull", this) as FrameworkElement;
            _dragTipFullAnimation.To = 0;
            highlight.BeginAnimation(OpacityProperty, _dragTipFullAnimation);
        }
        private void Filter_Drop(object sender, DragEventArgs e) {
            var highlight = Template.FindName("DragTipFull", this) as FrameworkElement;
            _dragTipFullAnimation.To = 0;
            highlight.BeginAnimation(OpacityProperty, _dragTipFullAnimation);
        }
    }
}
