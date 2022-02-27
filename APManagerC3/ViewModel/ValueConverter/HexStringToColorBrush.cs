using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace APManagerC3.ViewModel.ValueConverter {
    public class HexStringToColorBrush : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                string hex = (string)value;
                if (hex.StartsWith("#")) {
                    hex = hex[1..];
                }
                byte r = System.Convert.ToByte(hex[0..2], 16);
                byte g = System.Convert.ToByte(hex[2..4], 16);
                byte b = System.Convert.ToByte(hex[4..6], 16);
                return new SolidColorBrush(Color.FromRgb(r, g, b));
            } catch (Exception) {
                return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
