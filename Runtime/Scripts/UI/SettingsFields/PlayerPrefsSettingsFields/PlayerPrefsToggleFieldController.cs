using UnityEngine;

namespace CommonUtils.UI.SettingsFields.PlayerPrefsSettingsFields {
	public class PlayerPrefsToggleFieldController : AbstractToggleFieldController {
#pragma warning disable 649
		[SerializeField] private string playerPrefsKey;
		[SerializeField] private bool defaultValue;
#pragma warning restore 649

		protected override bool DefaultValue => PlayerPrefs.GetInt(playerPrefsKey, defaultValue ? 1 : 0) > 0;
		protected override void Save(bool value) => PlayerPrefs.SetInt(playerPrefsKey, value ? 1 : 0);
	}
}