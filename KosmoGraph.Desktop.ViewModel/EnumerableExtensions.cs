
namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> thisEnumerable, Action<T> applyToEach)
        {
            if (applyToEach != null)
                foreach (var item in thisEnumerable)
                    applyToEach(item);
        }
    }
}
