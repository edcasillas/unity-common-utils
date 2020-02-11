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
	}
}