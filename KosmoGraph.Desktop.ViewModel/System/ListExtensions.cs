namespace System.Collections
{
    using System;
    using System.Collections;

    public static class ListExtensions
    {
        public static IList ForEach(this IList thisList, Action<object> apply)
        {
            if (apply == null)
                return thisList;

            for (int i = 0; i < thisList.Count; i++)
                apply(thisList[i]);

            return thisList;
        }
    }
}
