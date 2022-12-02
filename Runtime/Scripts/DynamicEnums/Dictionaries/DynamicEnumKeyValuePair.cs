using System;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public abstract class DynamicEnumKeyValuePair<TValue>: ISerializationCallbackReceiver {
		[SerializeField] protected string enumName;
		public int Key;
		public TValue Value;

		// Inheritors can EnumName => "MyEnum" to create a key value pair where the key is of the type of the specified dynamic enum "MyEnum"
		public abstract string EnumName { get; }

		public void OnBeforeSerialize() => enumName = EnumName;

		public void OnAfterDeserialize() { }

		internal void SetEnumName(string newEnumName) => enumName = newEnumName;
	}

	/// <summary>
	/// Implements <see cref="DynamicEnumKeyValuePair{TValue}"/> to make <see cref="EnumName"/> always return the serialized
	/// value. The value of EnumName can then only be set with the SetEnumName method.
	/// </summary>
	[Serializable]
	public class AutoDynamicEnumKeyValuePair<TValue>: DynamicEnumKeyValuePair<TValue> { // TODO Rename!
		public override string EnumName => enumName;
	}
}
