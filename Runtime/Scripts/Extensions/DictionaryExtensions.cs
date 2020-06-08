using System;
using System.Collections.Generic;

namespace CommonUtils.Extensions {
	public static class DictionaryExtensions {
		public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
			if(dictionary==null) throw new ArgumentNullException(nameof(dictionary));
			if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
			else dictionary[key] = value;
		}
	}
}