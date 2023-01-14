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
		/// <param name="startIndex">First allowed index for the random result. Default is zero.</param>
		/// <param name="endIndex">Final allowed index for the random result. If not specified, defaults to the last index of the <paramref name="collection"/>.</param>
		/// <returns>Random item inside the collection, or its default value (e.g. null) if the collection is null or empty.</returns>
		public static T PickRandom<T>(this IEnumerable<T> collection, int startIndex = 0, int? endIndex = null) {
			var list = collection.ToList();
			return !list.IsNullOrEmpty() ?
				list.ElementAt(Random.Range(startIndex, endIndex ?? list.Count())) :
				default;
		}

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
		/// Adds all <paramref name="items"/> to the <paramref name="set"/> if they don't exist and omits duplicates.
		/// </summary>
		/// <param name="set">Hash set to be updated.</param>
		/// <param name="items">Collection of items to be added.</param>
		/// <typeparam name="T">Type of items to be added.</typeparam>
		public static void SafeAdd<T>(this HashSet<T> set, IEnumerable<T> items) {
			if (items.IsNullOrEmpty()) return;
			foreach (var item in items) {
				SafeAdd(set, item);
			}
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

		/// <summary>
		/// Gets the index with the minimum value from this <paramref name="collection"/>.
		/// </summary>
		public static int IndexOfMin(this IEnumerable<int> collection) {
			var minIndex = 0;
			for (var i = 1; i < collection.Count(); i++) {
				if (collection.ElementAt(i) < collection.ElementAt(minIndex)) minIndex = i;
			}

			return minIndex;
		}
	}
}