using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KosmoGraph.Desktop.ViewModel.Test
{
    public static class TestHelper
    {
        public static T2 With<T1, T2>( this T1 t, Func<T1,T2> doWith)
        {
            return doWith(t);
        }
    }
}
