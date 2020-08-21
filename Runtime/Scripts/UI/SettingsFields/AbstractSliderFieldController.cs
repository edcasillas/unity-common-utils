using UnityEngine.UI;

namespace CommonUtils.UI.SettingsFields {
	public abstract class AbstractSliderFieldController : AbstractSettingsFieldController<float, Slider> {
		protected override void Start() {
			Field.value = DefaultValue;
			if (AutoSave) Field.onValueChanged.AddListener(onValueChanged);
		}

		private void onValueChanged(float value) => Save(value);

		public sealed override void Save() => Save(Field.value);
	}
}