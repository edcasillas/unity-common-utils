using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.SettingsFields {
	public abstract class AbstractSettingsFieldController<TData, TField> : MonoBehaviour, ISettingsFieldController
		where TField : Selectable
	{
#pragma warning disable 649
		[SerializeField] protected bool AutoSave = true;
#pragma warning restore 649

		protected abstract TData DefaultValue { get; }

		private TField field;

		protected TField Field {
			get {
				if (!field) field = GetComponent<TField>();
				return field;
			}
		}

		protected abstract void Start();
		protected abstract void Save(TData value);
		public abstract    void Save();
	}
}