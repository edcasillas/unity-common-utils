using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public abstract class DynamicEnumDictionary<TValue> : ISerializationCallbackReceiver, IDictionary<int, TValue> {
		[SerializeField] private string enumName;
		[SerializeField] private List<AutoDynamicEnumKeyValuePair<TValue>> innerList;

		public abstract string EnumName { get; }

		private Dictionary<int, TValue> innerDictionary;

		public void MakeReadOnly() => innerDictionary = innerList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		#region Serialization
		public void OnBeforeSerialize() {
			if(innerList == null) return;

			var allValues = DynamicEnumManager.GetValues(EnumName);
			if (allValues == null) return;

			var enumValuesCount = allValues.Count;

			if (innerList.Count > enumValuesCount) {
				Debug.LogError($"Cannot add more values to the dictionary because the dynamic enum \"{EnumName}\" doesn't have enough values to create more unique keys.");
				innerList.RemoveRange(enumValuesCount, innerList.Count - enumValuesCount);
			}

			var usedKeys = new bool[enumValuesCount];
			var usedKeysCount = 0;

			enumName = EnumName;
			foreach (var kvp in innerList) {
				if (usedKeys[kvp.Key]) {
					kvp.Key = getNextAvailableKey(usedKeys);
				}
				kvp.SetEnumName(enumName);
				usedKeys[kvp.Key] = true;
				if(++usedKeysCount >= enumValuesCount) break;
			}
		}

		private int getNextAvailableKey(bool[] keys) {
			for (var i = 0; i < keys.Length; i++) {
				if (!keys[i]) return i;
			}

			return -1;
		}

		public void OnAfterDeserialize() { }
		#endregion

		#region Dictionary Implementation
		public bool IsReadOnly => innerDictionary != null;
		public ICollection<int> Keys => IsReadOnly ? innerDictionary.Keys : innerList.Select(kvp => kvp.Key).ToList();
		public ICollection<TValue> Values => IsReadOnly ? innerDictionary.Values : innerList.Select(kvp => kvp.Value).ToList();

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator() =>
			IsReadOnly ? innerDictionary.GetEnumerator() :
				innerList.Select(kvp => new KeyValuePair<int, TValue>(kvp.Key, kvp.Value)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool Contains(KeyValuePair<int, TValue> item) =>
			IsReadOnly ? innerDictionary.Contains(item) : innerList.Any(kvp => kvp.Key == item.Key);

		public bool ContainsKey(int key)
			=> IsReadOnly ? innerDictionary.ContainsKey(key) : innerList.Any(kvp => kvp.Key == key);

		public void Add(KeyValuePair<int, TValue> item) => Add(item.Key, item.Value);

		public void Add(int key, TValue value) {
			if (IsReadOnly) throw new InvalidOperationException("This dictionary has been made readonly.");

			if(ContainsKey(key)) throw new ArgumentException($"Key \"{key}\" already exists");
			innerList.Add(new AutoDynamicEnumKeyValuePair<TValue>(){Key = key, Value = value});
		}

		public void Clear() {
			if (IsReadOnly) throw new InvalidOperationException("This dictionary has been made readonly.");
			innerList.Clear();
		}

		public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex) => innerList.Select(kvp=>new KeyValuePair<int,TValue>(kvp.Key, kvp.Value)).ToList().CopyTo(array, arrayIndex);

		public bool Remove(KeyValuePair<int, TValue> item) => Remove(item.Key);

		public bool Remove(int key) {
			if (IsReadOnly) throw new InvalidOperationException("This dictionary has been made readonly.");
			var index = -1;
			for (int i = 0; i < innerList.Count; i++) {
				if (innerList[i].Key == key) {
					index = i;
					break;
				}
			}


			if (index >= 0) {
				innerList.RemoveAt(index);
				return true;
			}

			return false;
		}

		public int Count => IsReadOnly? innerDictionary.Count : innerList.Count;

		public bool TryGetValue(int key, out TValue value) {
			if (IsReadOnly) {
				return innerDictionary.TryGetValue(key, out value);
			}

			value = default;
			for (int i = 0; i < innerList.Count; i++) {
				if (innerList[i].Key == key) {
					value = innerList[i].Value;
					return true;
				}
			}

			return false;
		}

		public TValue this[int key] {
			get {
				if (IsReadOnly) {
					return innerDictionary[key];
				}

				foreach (var t in innerList.Where(t => t.Key == key)) {
					return t.Value;
				}

				throw new KeyNotFoundException($"Key {key}({DynamicEnumManager.IntToValue(enumName, key)}) was not found in the dictionary.");
			}
			set {
				if (IsReadOnly) throw new InvalidOperationException("This dictionary has been made readonly.");

				foreach (var t in innerList.Where(t => t.Key == key)) {
					t.Value = value;
				}

				throw new KeyNotFoundException($"Key {key} was not found in the dictionary.");
			}
		}
		#endregion
	}
}