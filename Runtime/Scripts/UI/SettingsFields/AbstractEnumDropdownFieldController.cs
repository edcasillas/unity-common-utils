using System;
using System.Collections;
using System.Text.RegularExpressions;
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
				var valueToText = Regex.Replace(((TEnum) values.GetValue(i)).ToString(), "([A-Z0-9])([a-z]*)", " $1$2");
				var opt  = new Dropdown.OptionData { text = valueToText };
				Field.options.Add(opt);
			}

			Field.value = Convert.ToInt32(DefaultValue);

			if(AutoSave) Field.onValueChanged.AddListener(onValueChanged);

			Field.RefreshShownValue();
		}

		private void onValueChanged(int value) => Save((TEnum)Enum.Parse(typeof(TEnum), value.ToString()));

		public sealed override void Save() => Save((TEnum)Enum.Parse(typeof(TEnum), Field.value.ToString()));
	}
}