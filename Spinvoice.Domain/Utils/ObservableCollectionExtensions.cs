using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spinvoice.Domain.Utils
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> items, IEnumerable<T>  itemsToAdd)
        {
            itemsToAdd.ForEach(items.Add);
        }
    }
}
