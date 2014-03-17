using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KosmoGraph.Desktop.View
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> AscendLogicalTree(this DependencyObject from)
        {
            yield return LogicalTreeHelper.GetParent(from);
        }

        public static IEnumerable<DependencyObject> AscendVisualTree(this DependencyObject from)
        {
            yield return VisualTreeHelper.GetParent(from);
        }

        #region //Walk up to the paremet of a dependency object in a visual or logical tree.

        //public static T GetLogicalParentOfType<T>(this DependencyObject child)
        //    where T : DependencyObject
        //{
        //    return getParentOfType<T>(child, LogicalTreeHelper.GetParent);
        //}
        
        //public static T GetVisualParentOfType<T>(this DependencyObject child)
        //    where T : DependencyObject
        //{
        //    return getParentOfType<T>(child, VisualTreeHelper.GetParent);
        //}
        
        //private static T getParentOfType<T>(DependencyObject child, Func<DependencyObject, DependencyObject> getParent)
        //    where T : DependencyObject
        //{
        //    DependencyObject searchedItem = child;

        //    while (searchedItem != null && !(searchedItem is T))
        //        searchedItem = getParent(searchedItem);

        //    return searchedItem as T;
        //}

        #endregion
    }
}
