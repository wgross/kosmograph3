namespace KosmoGraph.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public static class ExecutedRoutedEventArgsExtensions
    {
        public static TData GetParameter<TData>(this ExecutedRoutedEventArgs args)
        {
            return (TData)args.Parameter;
        }
    }
}
