using System;
using UnityEngine;

namespace CommonUtils.DynamicEnums.Dictionaries {
	[Serializable]
	public class DynamicEnumKeyValuePair<TValue> {
		[SerializeField] private int key;
		[SerializeField] private TValue value;
	}
}
