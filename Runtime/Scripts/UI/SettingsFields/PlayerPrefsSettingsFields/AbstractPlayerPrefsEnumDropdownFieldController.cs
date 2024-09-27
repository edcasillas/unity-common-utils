using System;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.UI.SettingsFields.PlayerPrefsSettingsFields {
	/// <summary>
	/// Controls a drop down field that is connected to a PlayerPrefs key storing an enum value.
	/// To use it, create a subclass to specify the type of enum, then attach the inherited class to a game object
	/// with a Dropdown component.
	/// </summary>
	/// <typeparam name="TEnum">Type of enum handled by this drop down list.</typeparam>
	public class AbstractPlayerPrefsEnumDropdownFieldController<TEnum> : AbstractEnumDropdownFieldController<TEnum>
		where TEnum : Enum {
		[SerializeField] private string playerPrefsKey;
		[SerializeField] private TEnum defaultValue;

		protected override TEnum DefaultValue => PlayerPrefs.GetInt(playerPrefsKey, defaultValue.ToInt()).ToEnumValue<TEnum>();

		protected override void Save(TEnum value) {
			PlayerPrefs.SetInt(playerPrefsKey, value.ToInt());
			PlayerPrefs.Save();
		}
	}
}