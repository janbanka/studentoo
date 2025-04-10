using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace studentoo
{
    class IntToVisibilityConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                bool shouldCollapse = parameter?.ToString()?.Equals("Collapse", StringComparison.OrdinalIgnoreCase) ?? true;

                return count > 0
                    ? Visibility.Visible
                    : (shouldCollapse ? Visibility.Collapsed : Visibility.Hidden);
            }
            return Visibility.Collapsed; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible ? 1 : 0;
            }
            return 0;
        }
    }
}
