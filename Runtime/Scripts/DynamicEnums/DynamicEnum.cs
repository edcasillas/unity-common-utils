using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Inspector.ReorderableInspector;
using SubjectNerd.Utilities;
using UnityEngine;

namespace CommonUtils.DynamicEnums {
	[Serializable]
	public class DynamicEnum {
#pragma warning disable 649
		[SerializeField] private string name;
		[SerializeField] [Reorderable] private string[] values;
#pragma warning restore 649

		public string Name => name;

		public IReadOnlyCollection<string> Values => values;

		private GUIContent[] valuesAsGuiContent;
		public GUIContent[] ValuesAsGuiContent {
			get {
				if (valuesAsGuiContent == null || valuesAsGuiContent.Length != values.Length) {
					loadValuesAsGuiContent();
				}
				return valuesAsGuiContent;
			}
		}

		private Dictionary<string, int> _valuesToInt;

		private Dictionary<string, int> valuesToInt {
			get {
				if (_valuesToInt == null) loadValuesToInt();
				return _valuesToInt;
			}
		}


		public bool Contains(string value) => valuesToInt.ContainsKey(value);

		public int ToInt(string value) => valuesToInt.TryGetValue(value, out var result) ? result : -1;

		public string ToValue(int index) => index < values.Length ? values[index] : null;

		public void Reload() {
			_valuesToInt = null;
			valuesAsGuiContent = null;
		}

		private void loadValuesToInt() {
			_valuesToInt = new Dictionary<string, int>();
			for (var i = 0; i < values.Length; i++) {
				_valuesToInt.Add(values[i], i);
			}
		}

		private void loadValuesAsGuiContent() => valuesAsGuiContent = values.Select(v => new GUIContent(v)).ToArray();
	}
}