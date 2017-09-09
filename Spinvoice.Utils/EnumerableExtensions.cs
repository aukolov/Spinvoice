using System;
using System.Collections.Generic;

namespace Spinvoice.Utils
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static T MinBy<T, TProperty>(
            this IEnumerable<T> items,
            Func<T, TProperty> selector)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var minFound = false;
            TProperty currentMin = default(TProperty);
            T currentItem = default(T);
            var comparer = Comparer<TProperty>.Default;
            foreach (var item in items)
            {
                var value = selector(item);
                if (!minFound)
                {
                    minFound = true;
                    currentMin = value;
                    currentItem = item;
                }
                else
                {
                    if (comparer.Compare(value, currentMin) < 0)
                    {
                        currentMin = value;
                        currentItem = item;
                    }
                }
            }

            if (!minFound)
            {
                throw new InvalidOperationException("Enumerable does not have any items.");
            }
            return currentItem;
        }
    }
}