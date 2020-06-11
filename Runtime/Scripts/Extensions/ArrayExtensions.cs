using System;

namespace CommonUtils.Extensions {
	public static class ArrayExtensions {
		/// <summary>
		/// Returns a copy of the <paramref name="source"/> array without the item at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="source">Array to copy.</param>
		/// <param name="index">Index of the item to remove.</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] RemoveAt<T>(this T[] source, int index) {
			var dest = new T[source.Length - 1];
			if (index > 0)
				Array.Copy(source, 0, dest, 0, index);

			if (index < source.Length - 1)
				Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

			return dest;
		}

		/// <summary>
		/// Performs a binary search in the <paramref name="source"/> array looking for the <paramref name="target"/> value.
		/// If found, returns its index; if not found, returns the closest index to the left of that value.
		/// </summary>
		public static int BinarySearchClosestLowerIndex(this float[] source, float target) {
			var start = 0;
			var end = source.Length;

			while (start < end - 1) {
				var mid = (start + end) / 2;
				if (target == source[mid]) {
					return mid;
				}

				if (target < source[mid]) end = mid;
				else start = mid;
			}

			return start;
		}
	}
}