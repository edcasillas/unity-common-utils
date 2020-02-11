using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class EnumerableExtensions {
		/// <summary>
		/// Picks a random item from this collection.
		/// </summary>
		/// <typeparam name="T">Type of items inside the collection.</typeparam>
		/// <param name="collection">Collection of items from which to pick a random item.</param>
		/// <returns>Random item inside the collection, or its default value (e.g. null) if the collection is null or empty.</returns>
		public static T PickRandom<T>(this IEnumerable<T> collection) =>
			!collection.IsNullOrEmpty() ?
				collection.ElementAt(Random.Range(0, collection.Count())) :
				default(T);

		/// <summary>
		/// Gets a value indicating whether this collection is null or empty.
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

		/// <summary>
		/// Adds <paramref name="item"/> to the <paramref name="set"/> if it doesn't exist already.
		/// </summary>
		/// <param name="set">Hash set to be updated</param>
		/// <param name="item">Item to insert</param>
		/// <typeparam name="T">Type of item to insert.</typeparam>
		public static void SafeAdd<T>(this HashSet<T> set, T item) {
			if (!set.Contains(item)) set.Add(item);
		}

		/// <summary>
		/// Removes <paramref name="item"/> from the <paramref name="set"/> if it exists.
		/// </summary>
		/// <param name="set">Hash set to be updated.</param>
		/// <param name="item">Item to remove</param>
		/// <typeparam name="T">Type of item to remove.</typeparam>
		public static void SafeRemove<T>(this HashSet<T> set, T item) {
			if (set.Contains(item)) set.Remove(item);
		}
	}
}