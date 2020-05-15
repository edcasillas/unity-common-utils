using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.Serializables { // Based on this answer: http://answers.unity.com/answers/809221/view.html
	[Serializable]
	public abstract class AbstractSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
		[SerializeField]
		private List<TKey> keys = new List<TKey>();

		[SerializeField]
		private List<TValue> values = new List<TValue>();

		public AbstractSerializableDictionary() : base() { }

		public AbstractSerializableDictionary(IDictionary<TKey, TValue> source) : base(source) { }

		public void OnBeforeSerialize() {
			keys.Clear();
			values.Clear();
			foreach (var pair in this) {
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		public void OnAfterDeserialize() {
			Clear();

			while (values.Count > keys.Count) {
				Debug.LogError(
					$"Value \"{values[values.Count - 1]}\" was removed from the serializable dictionary because it had no matching key.");
				values.RemoveAt(values.Count - 1);
			}

			while (keys.Count > values.Count) {
				values.Add(default);
			}

			for (var i = 0; i < keys.Count; i++)
				Add(keys[i], values[i]);
		}
	}
}