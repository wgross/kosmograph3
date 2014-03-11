namespace KosmoGraph.Desktop.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public static readonly IValueConverter True2Visible = new BooleanToVisibilityConverter { IsNegate = false };

        public static readonly IValueConverter False2Visible = new BooleanToVisibilityConverter { IsNegate = true };

        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
                return (object)this.BoolToVisibility((bool)value);
            else if (targetType == typeof(bool))
                return (object)this.VisibilityToBool((Visibility)value);
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
                return (object)this.BoolToVisibility((bool)value);
            else if (targetType == typeof(bool))
                return (object)this.VisibilityToBool((Visibility)value);
            else
                return DependencyProperty.UnsetValue;
        }

        #endregion IValueConverter Member

        #region Conversion logic

        private bool IsNegate { get; set; }

        public bool VisibilityToBool(Visibility value)
        {
            if (this.IsNegate)
                return value == Visibility.Visible ? false : true;
            else
                return value == Visibility.Visible ? true : false;
        }

        public Visibility BoolToVisibility(bool value)
        {
            if (this.IsNegate)
                return value ? Visibility.Collapsed : Visibility.Visible;
            else
                return value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion Conversion logic
    }
}
