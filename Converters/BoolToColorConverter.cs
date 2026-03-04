using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NotepadPlusPlus.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is true
                ? new SolidColorBrush(Color.FromRgb(220, 80, 80))
                : new SolidColorBrush(Colors.Black);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}