namespace KosmoGraph.Desktop.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public static class AttachedActualSizeBindingBehaviour
    {
        public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
            "Observe",
            typeof(bool),
            typeof(AttachedActualSizeBindingBehaviour),
            new FrameworkPropertyMetadata(OnObserveChanged));

        public static readonly DependencyProperty ObservedActualWidthProperty = DependencyProperty.RegisterAttached(
            "ObservedActualWidth",
            typeof(double),
            typeof(AttachedActualSizeBindingBehaviour));

        public static readonly DependencyProperty ObservedActualHeightProperty = DependencyProperty.RegisterAttached(
            "ObservedActualHeight",
            typeof(double),
            typeof(AttachedActualSizeBindingBehaviour));

        public static bool GetObserve(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ObserveProperty);
        }

        public static void SetObserve(FrameworkElement frameworkElement, bool observe)
        {
            frameworkElement.SetValue(ObserveProperty, observe);
        }

        public static double GetObservedActualWidth(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(ObservedActualWidthProperty);
        }

        public static void SetObservedActualWidth(FrameworkElement frameworkElement, double observedActualWidth)
        {
            frameworkElement.SetValue(ObservedActualWidthProperty, observedActualWidth);
        }

        public static double GetObservedActualHeight(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(ObservedActualHeightProperty);
        }

        public static void SetObservedActualHeight(FrameworkElement frameworkElement, double observedActualHeight)
        {
            frameworkElement.SetValue(ObservedActualHeightProperty, observedActualHeight);
        }

        private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)dependencyObject;

            if ((bool)e.NewValue)
            {
                frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
            else
            {
                frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
            }
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateObservedSizesForFrameworkElement((FrameworkElement)sender);
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
        {
            frameworkElement.SetCurrentValue(ObservedActualWidthProperty, frameworkElement.ActualWidth);
            frameworkElement.SetCurrentValue(ObservedActualHeightProperty, frameworkElement.ActualHeight);
        }
    }
}
