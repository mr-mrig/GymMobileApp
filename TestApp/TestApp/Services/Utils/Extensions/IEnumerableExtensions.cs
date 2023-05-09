using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestApp.Services.Utils.Extensions
{
    public static class IEnumerableExtensions
    {

        /// <summary>
        /// Checks whether the IEnumerable contains duplicate objects
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <returns>True if tere are duplicates</returns>
        public static bool ContainsDuplicates<T>(this IEnumerable<T> list)

            => !list.All(new HashSet<T>().Add);

        /// <summary>
        /// Checks whether the IEnumerable contains duplicate objects with respect to the specified selector
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <param name="selector">The selection function</param>
        /// <returns>True if tere are duplicates</returns>
        public static bool ContainsDuplicates<T1,T2>(this IEnumerable<T1> list, Func<T1, T2> selector)
        {
            var d = new HashSet<T2>();
            foreach (var t in list)
            {
                if (!d.Add(selector(t)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the IEnumerable contains duplicate objects with respect to the specified selector
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <param name="selector">The selection function</param>
        /// <returns>True if tere are duplicates</returns>
        public static bool ContainsDuplicates<T>(this IEnumerable list, Func<object, T> selector)
        {
            var d = new HashSet<T>();
            foreach (var t in list)
            {
                if (!d.Add(selector(t)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the IEnumerable is just a repetition of a single element
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <returns>True if there are duplicates</returns>
        public static bool HasOnlyOneDistinctValue<T>(this IEnumerable<T> list)
        {
            return list.Distinct().Count() == 1;
        }

        /// <summary>
        /// Perform the SequenceEqual operation over the list checking the items in a grouped fashion
        /// IE: {10, 8, 6, 10, 8, 6} returns true when grouped 3-by-3
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <param name="groupCounter">The number of items in each group</param>
        /// <returns></returns>
        public static bool GroupedSequenceEqual<T>(this IEnumerable<T> list, int groupCounter)
        {
            int iGroup = 0;

            do
            {
                if (!list.Where((x, i) => ((i + iGroup) % groupCounter) == 0).HasOnlyOneDistinctValue())
                    return false;
            }
            while (++iGroup < groupCounter);

            return true;
        }

        /// <summary>
        /// Perform the SequenceEqual operation by using the specified Predicate to compare the items
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <param name="other">The list to check against</param>
        /// <param name="compareBy">The predicate as a Func used as a comparator</param>
        /// <returns>The SequenceEqual result</returns>
        //public static bool SequenceEqual<T>(this IEnumerable<T> list, IEnumerable<T> other, Func<T, bool> compareBy)
        //{
        //    return list.Select(compareBy).SequenceEqual(other.Select(compareBy));
        //}

        /// <summary>
        /// Perform the SequenceEqual operation on a subset of items.
        /// </summary>
        /// <typeparam name="T">The IEnumerable objects type</typeparam>
        /// <param name="list"></param>
        /// <param name="other">The list to check against</param>
        /// <param name="startingFromIndex">The zero-based index to start comparing from</param>
        /// <param name="numberOfItems">The number of items to be compared</param>
        /// <returns>The SequenceEqual result</returns>
        public static bool SequenceEqual<T>(this IEnumerable<T> list, IEnumerable<T> other, int startingFromIndex, int numberOfItems)
            => list.Skip(startingFromIndex).Take(numberOfItems).SequenceEqual(
                other.Skip(startingFromIndex).Take(numberOfItems));
        //public static bool SequenceEqual<T>(this IEnumerable<T> list, IEnumerable<T> other, int startingFromIndex, int numberOfItems)
        //{
        //    IEnumerator<T> thisEnumerator = list.GetEnumerator();
        //    IEnumerator<T> otherEnumerator = other.GetEnumerator();
        //    int counter = 0;
        //    int endToIndex = startingFromIndex + numberOfItems;
        //    bool result = true;

        //    while(thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
        //    {
        //        if (counter >= startingFromIndex)
        //        {
        //            if(counter <= endToIndex)
        //            {
        //                if (!EqualityComparer<T>.Default.Equals(thisEnumerator.Current, otherEnumerator.Current))
        //                    return false;
        //            }
        //            break;
        //        }
        //        counter++;
        //    }
        //    return result;
        //}

    }
}
