using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestApp.Services.Utils.Extensions
{
    public static class ObservableCollectionExtensions
    {

        /// <summary>
        /// Remove all the elements that match the specified condition
        /// </summary>
        /// <typeparam name="T">The type of the objects stored</typeparam>
        /// <param name="items">The input list</param>
        /// <param name="condition">The selection predicate</param>
        /// <returns>The number of items removed</returns>
        public static int RemoveAll<T>(this ObservableCollection<T> items, Func<T, bool> condition)
        {
            List<T> itemsToRemove = items.Where(condition).ToList();

            foreach (T itemToRemove in itemsToRemove)
                items.Remove(itemToRemove);

            return itemsToRemove.Count;
        }
    }
}
