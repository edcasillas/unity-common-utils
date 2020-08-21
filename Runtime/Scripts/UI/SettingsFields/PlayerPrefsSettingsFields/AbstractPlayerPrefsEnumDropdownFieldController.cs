using System;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.UI.SettingsFields.PlayerPrefsSettingsFields {
	public class AbstractPlayerPrefsEnumDropdownFieldController<TEnum> : AbstractEnumDropdownFieldController<TEnum>
		where TEnum : Enum {
#pragma warning disable 649
		[SerializeField] private string playerPrefsKey;
		[SerializeField] private TEnum defaultValue;
#pragma warning restore 649

		protected override TEnum DefaultValue => PlayerPrefs.GetInt(playerPrefsKey, defaultValue.ToInt()).ToEnumValue<TEnum>();

		protected override void Save(TEnum value) {
			PlayerPrefs.SetInt(playerPrefsKey, value.ToInt());
			PlayerPrefs.Save();
		}
	}
}