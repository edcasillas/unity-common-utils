using System;
using System.Collections.Generic;
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
	public class TestKVP<TValue>: DynamicEnumKeyValuePair<TValue> {
		public override string EnumName => enumName;
	}

	[Serializable]
	public abstract class DynamicEnumDictionary<TValue> : ISerializationCallbackReceiver {
		[SerializeField] private string enumName;
		[SerializeField] private List<TestKVP<TValue>> kvps;

		public abstract string EnumName { get; }

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

		public void OnAfterDeserialize() {

		}
	}
}
