using System.Windows;
using System.Windows.Controls;

namespace APManagerC3.View {
    /// <summary>
    /// RGBValueSlider.xaml 的交互逻辑
    /// </summary>
    [TemplatePart(Name = "PART_RGBLabel", Type = typeof(Label))]
    public partial class RGBValueSlider : Slider {
        public static readonly DependencyProperty RGBLabelProperty =
            DependencyProperty.Register(nameof(RGBLabel), typeof(string), typeof(RGBValueSlider), new PropertyMetadata(""));

        public string RGBLabel {
            get {
                return (string)GetValue(RGBLabelProperty);
            }
            set {
                SetValue(RGBLabelProperty, value);
            }
        }
    }
}
