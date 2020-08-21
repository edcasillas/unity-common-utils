using System;
using System.Collections;
using System.Text.RegularExpressions;
using CommonUtils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.SettingsFields {
	[RequireComponent(typeof(Dropdown))]
	public abstract class AbstractEnumDropdownFieldController<TEnum> : AbstractSettingsFieldController<TEnum, Dropdown>
		where TEnum : Enum
	{
		protected override void Start() => StartCoroutine(loadOptions());

		private IEnumerator loadOptions() {
			Field.options.Clear();
			yield return null;

			var values = typeof(TEnum).GetEnumValues();
			for (int i = 0; i < values.Length; i++) {
				var valueToText = ((TEnum)values.GetValue(i)).ToText();
				var opt  = new Dropdown.OptionData { text = valueToText };
				Field.options.Add(opt);
			}

			Field.value = DefaultValue.ToInt();

			if(AutoSave) Field.onValueChanged.AddListener(onValueChanged);

			Field.RefreshShownValue();
		}

		private void onValueChanged(int value) => Save(value.ToEnumValue<TEnum>());

		public sealed override void Save() => Save(Field.value.ToEnumValue<TEnum>());
	}
}