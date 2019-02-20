namespace SharedCommon.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class IListExtensions
    {
        public static IList<T> ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                action(item);
            }

            return list;
        }
    }
}