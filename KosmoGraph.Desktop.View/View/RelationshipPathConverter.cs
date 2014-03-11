namespace KosmoGraph.Desktop.View
{ 
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(List<Point>), typeof(PathSegmentCollection))]
    public sealed class RelationshipPathConverter : IValueConverter
    {
        public static RelationshipPathConverter Singleton = new RelationshipPathConverter();
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Point> points = (List<Point>)value;
            PointCollection pointCollection = new PointCollection();
            foreach (Point point in points)
            {
                pointCollection.Add(point);   
            }
            return pointCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
