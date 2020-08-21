using UnityEngine;

namespace CommonUtils.UI.SettingsFields.PlayerPrefsSettingsFields {
	public class PlayerPrefsSliderFieldController : AbstractSliderFieldController {
#pragma warning disable 649
		[SerializeField] private string playerPrefsKey;
		[SerializeField] private float defaultValue;
#pragma warning restore 649
		protected override float DefaultValue => PlayerPrefs.GetFloat(playerPrefsKey, defaultValue);
		protected override void Save(float value) => PlayerPrefs.SetFloat(playerPrefsKey, value);
	}
}