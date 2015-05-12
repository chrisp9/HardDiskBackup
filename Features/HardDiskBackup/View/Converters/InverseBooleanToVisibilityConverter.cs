using System;
using System.Windows;
using System.Windows.Data;

namespace HardDiskBackup.View.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolVal = value as bool?;
            if (boolVal == null)
                return Visibility.Visible;

            return boolVal.Value ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}