using System.Windows;
using System.Windows.Controls;

namespace APManagerC3.View {
    /// <summary>
    /// FileDragControl.xaml 的交互逻辑
    /// </summary>
    public partial class FileDragControl : UserControl {
        public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register(nameof(Tip), typeof(string), typeof(FileDragControl), new PropertyMetadata("拖拽至此载入"));

        public event DragEventHandler FileDraged;
        public string Tip {
            get { return (string)GetValue(TipProperty); }
            set { SetValue(TipProperty, value); }
        }

        public FileDragControl() {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e) {
            FileDraged?.Invoke(this, e);
        }
    }
}
