using System;
using System.Collections.Generic;

namespace Onsharp.Utils
{
    /// <summary>
    /// LinqExtensions offer utility functions for linq.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Makes a for each safe via reverse iterating. Modifying the list won't end in an exception.
        /// </summary>
        /// <param name="list">The list to be iterated</param>
        /// <param name="callback">The callback which gets called on iterate, returning true ends in a break</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        public static void SafeForEach<T>(this IReadOnlyList<T> list, Func<T, bool> callback)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if(callback.Invoke(list[i]))
                    break;
            }
        }
        /// <summary>
        /// Makes a for each safe via reverse iterating. Modifying the list won't end in an exception.
        /// </summary>
        /// <param name="list">The list to be iterated</param>
        /// <param name="callback">The callback which gets called on iterate, returning true ends in a break</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        public static void SafeForEach<T>(this List<T> list, Func<T, bool> callback)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if(callback.Invoke(list[i]))
                    break;
            }
        }

        /// <summary>
        /// Makes a for each safe via reverse iterating. Modifying the list won't end in an exception.
        /// </summary>
        /// <param name="list">The list to be iterated</param>
        /// <param name="callback">The callback which gets called on iterate</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        public static void SafeForEach<T>(this IReadOnlyList<T> list, Action<T> callback)
        {
            list.SafeForEach(item =>
            {
                callback.Invoke(item);
                return false;
            });
        }

        /// <summary>
        /// Makes a for each safe via reverse iterating. Modifying the list won't end in an exception.
        /// </summary>
        /// <param name="list">The list to be iterated</param>
        /// <param name="callback">The callback which gets called on iterate</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        public static void SafeForEach<T>(this List<T> list, Action<T> callback)
        {
            list.SafeForEach(item =>
            {
                callback.Invoke(item);
                return false;
            });
        }

        /// <summary>
        /// Selects all items fitting to the given predicate.
        /// </summary>
        /// <param name="list">The list to be checked</param>
        /// <param name="check">The predicate which filters</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        /// <returns>The list containing all fitting elements</returns>
        public static List<T> SelectAll<T>(this IReadOnlyList<T> list, Predicate<T> check)
        {
            List<T> newList = new List<T>();
            list.SafeForEach(item =>
            {
                if(check.Invoke(item))
                    newList.Add(item);
            });
            return newList;
        }

        /// <summary>
        /// Selects all items fitting to the given predicate.
        /// </summary>
        /// <param name="list">The list to be checked</param>
        /// <param name="check">The predicate which filters</param>
        /// <typeparam name="T">The type of the list elements</typeparam>
        /// <returns>The list containing all fitting elements</returns>
        public static List<T> SelectAll<T>(this List<T> list, Predicate<T> check)
        {
            List<T> newList = new List<T>();
            list.SafeForEach(item =>
            {
                if(check.Invoke(item))
                    newList.Add(item);
            });
            return newList;
        }
    }
}