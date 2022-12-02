using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public abstract class DynamicEnumKeyValuePair<TValue>: ISerializationCallbackReceiver {
		[SerializeField] private string enumName;
		[SerializeField] private int key;
		[SerializeField] private TValue value;

		public abstract string EnumName { get; }

		public void OnBeforeSerialize() => enumName = EnumName;

		public void OnAfterDeserialize() { }
	}

	[Serializable]
	public class TestKVP<TValue>: DynamicEnumKeyValuePair<TValue> {
		public override string EnumName => "Platform";
	}

	[Serializable]
	public class DynamicEnumDictionary {
		[SerializeField] private List<TestKVP<GameObject>> kvps;
	}
}
