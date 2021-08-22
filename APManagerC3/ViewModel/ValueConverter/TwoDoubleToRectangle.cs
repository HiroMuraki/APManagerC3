﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace APManagerC3.ViewModel.ValueConverter {
    public class TwoDoubleToRectangle : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            try {
                double width = (double)values[0];
                double height = (double)values[1];
                return new Rect(0, 0, width, height);
            }
            catch (Exception) {
                throw;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
