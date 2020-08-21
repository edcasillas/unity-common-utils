using UnityEngine.UI;

namespace CommonUtils.UI.SettingsFields {
	public abstract class AbstractToggleController : AbstractSettingsFieldController<bool, Toggle> {
		protected override void Start() {
			Field.isOn = DefaultValue;
			if (AutoSave) Field.onValueChanged.AddListener(onValueChanged);
		}

		private void onValueChanged(bool value) => Save(value);

		public sealed override void Save() => Save(Field.isOn);
	}
}