namespace KosmoGraph.Desktop.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public static readonly IValueConverter Null2Collapsed = new NullToVisibilityConverter();

        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }

        #endregion IValueConverter Member
    }
}
