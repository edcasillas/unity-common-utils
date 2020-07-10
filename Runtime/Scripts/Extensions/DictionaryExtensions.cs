using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils.Extensions {
	public static class DictionaryExtensions {
		public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
			if(dictionary==null) throw new ArgumentNullException(nameof(dictionary));
			if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
			else dictionary[key] = value;
		}

		public static string AsJsonString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) => $"{{{string.Join(", ", dictionary.Select(kvp => $"{{{kvp.Key.ToString()}, {kvp.Value.ToString()}}}"))}}}";
	}
}