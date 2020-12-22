using System;
using System.Collections.Generic;

namespace GDLibrary.Utilities
{
    public class CollectionUtility
    {
        /// <summary>
        /// Modifies an enumerable collection (e.g. List, Stack, Queue) using a filter and a transform function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">An enumerable collection containing objects of type T</param>
        /// <param name="filter">A filter predicate for an object of type T</param>
        /// <param name="transform">An action which will transform the object of type T</param>
        /// <returns>Integer count of the number of transformed objects</returns>
        public static int Transform<T>(IEnumerable<T> collection,
            Predicate<T> filter, Action<T> transform)
        {
            int count = 0;
            foreach (T obj in collection)
            {
                if (filter(obj))
                {
                    transform(obj);
                    count++;
                }
            }
            return count;
        }
    }
}