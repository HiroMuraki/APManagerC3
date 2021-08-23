using System;
using System.Windows;
using System.Windows.Controls;

namespace APManagerC3.View {
    public class ColorPicker : Control {
        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, OnRGBValueChanged));
        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, OnRGBValueChanged));
        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, OnRGBValueChanged));
        public static readonly DependencyProperty HexValueProperty =
            DependencyProperty.Register(nameof(HexValue), typeof(string), typeof(ColorPicker), new PropertyMetadata("", OnHexValueChanged));

        public byte R {
            get { return (byte)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }
        public byte G {
            get { return (byte)GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }
        public byte B {
            get { return (byte)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }
        public string HexValue {
            get { return (string)GetValue(HexValueProperty); }
            set { SetValue(HexValueProperty, value); }
        }

        private static void OnHexValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ColorPicker cp = d as ColorPicker;
            string hexValue = cp.HexValue;
            if (hexValue.StartsWith("#")) {
                hexValue = hexValue[1..];
            }
            if (hexValue.Length != 6) {
                cp.R = 0xff;
                cp.G = 0xff;
                cp.B = 0xff;
            }
            cp.R = Convert.ToByte(hexValue[0..2], 16);
            cp.G = Convert.ToByte(hexValue[2..4], 16);
            cp.B = Convert.ToByte(hexValue[4..6], 16);

        }
        private static void OnRGBValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ColorPicker cp = d as ColorPicker;
            cp.HexValue = $"{cp.R:X2}{cp.G:X2}{cp.B:X2}";
        }

        static ColorPicker() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }
    }
}
