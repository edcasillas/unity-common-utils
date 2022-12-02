using System;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public class DynamicEnumKeyValuePair<TValue>: ISerializationCallbackReceiver {
		[SerializeField] private string enumName;
		[SerializeField] private int key;
		[SerializeField] private TValue value;

		public string EnumName => "Platform";

		public void OnBeforeSerialize() => enumName = EnumName;

		public void OnAfterDeserialize() { }
	}
}
