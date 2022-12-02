using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public abstract class DynamicEnumKeyValuePair<TValue>: ISerializationCallbackReceiver {
		[SerializeField] protected string enumName;
		public int Key;
		public TValue Value;

		public abstract string EnumName { get; }

		public void OnBeforeSerialize() => enumName = EnumName;

		public void OnAfterDeserialize() { }

		internal void SetEnumName(string newEnumName) => enumName = newEnumName;
	}

	[Serializable]
	public class TestKVP<TValue>: DynamicEnumKeyValuePair<TValue> { // TODO Rename!
		public override string EnumName => enumName;
	}

	[Serializable]
	public abstract class DynamicEnumDictionary<TValue> : ISerializationCallbackReceiver, IDictionary<int, TValue> {
		[SerializeField] private string enumName;
		[SerializeField] private List<TestKVP<TValue>> kvps;

		public abstract string EnumName { get; }

		#region Serialization
		public void OnBeforeSerialize() {
			var enumValuesCount = DynamicEnumManager.GetValues(EnumName).Count;

			if (kvps.Count > enumValuesCount) {
				Debug.LogError($"Cannot add more values to the dictionary because the dynamic enum \"{EnumName}\" doesn't have enough values to create more unique keys.");
				kvps.RemoveRange(enumValuesCount, kvps.Count - enumValuesCount);
			}

			var usedKeys = new bool[enumValuesCount];
			var usedKeysCount = 0;

			enumName = EnumName;
			foreach (var kvp in kvps) {
				if (usedKeys[kvp.Key]) {
					kvp.Key = getNextAvailableKey(usedKeys);
				}
				kvp.SetEnumName(enumName);
				usedKeys[kvp.Key] = true;
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
		public ICollection<int> Keys => kvps.Select(kvp => kvp.Key).ToList();
		public ICollection<TValue> Values => kvps.Select(kvp => kvp.Value).ToList();

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator() {
			return kvps.Select(kvp => new KeyValuePair<int, TValue>(kvp.Key, kvp.Value)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool Contains(KeyValuePair<int, TValue> item) { return kvps.Any(kvp => kvp.Key == item.Key); }

		public bool ContainsKey(int key) { return kvps.Any(kvp => kvp.Key == key); }

		public void Add(KeyValuePair<int, TValue> item) {
			if (ContainsKey(item.Key)) throw new ArgumentException($"Key \"{item.Key}\" already exists");
			kvps.Add(new TestKVP<TValue>(){Key = item.Key, Value = item.Value});
		}

		public void Add(int key, TValue value) {
			if(ContainsKey(key)) throw new ArgumentException($"Key \"{key}\" already exists");
			kvps.Add(new TestKVP<TValue>(){Key = key, Value = value});
		}

		public void Clear() => kvps.Clear();
		public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex) {
			kvps.Select(kvp=>new KeyValuePair<int,TValue>(kvp.Key, kvp.Value)).ToList().CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<int, TValue> item) => Remove(item.Key);

		public bool Remove(int key) {
			var index = -1;
			for (int i = 0; i < kvps.Count; i++) {
				if (kvps[i].Key == key) {
					index = i;
					break;
				}
			}


			if (index >= 0) {
				kvps.RemoveAt(index);
				return true;
			}

			return false;
		}

		public int Count => kvps.Count;
		public bool IsReadOnly => false;

		public bool TryGetValue(int key, out TValue value) {
			value = default;
			for (int i = 0; i < kvps.Count; i++) {
				if (kvps[i].Key == key) {
					value = kvps[i].Value;
					return true;
				}
			}

			return false;
		}

		public TValue this[int key] {
			get {
				for (int i = 0; i < kvps.Count; i++) {
					if (kvps[i].Key == key) {
						return kvps[i].Value;
					}
				}

				throw new KeyNotFoundException($"Key {key} was not found in the dictionary.");
			}
			set {
				for (int i = 0; i < kvps.Count; i++) {
					if (kvps[i].Key == key) {
						kvps[i].Value = value;
					}
				}

				throw new KeyNotFoundException($"Key {key} was not found in the dictionary.");
			}
		}
		#endregion
	}
}
