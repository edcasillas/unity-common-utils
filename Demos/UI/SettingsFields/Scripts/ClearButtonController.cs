using CommonUtils.Inspector.ReorderableInspector;
using UnityEngine;

namespace Demos.UI.SettingsFields {
	public class ClearButtonController : MonoBehaviour {
#pragma warning disable 649
		[SerializeField] [Reorderable] private string[] keys;
#pragma warning restore 649

		public void OnPressed() {
			foreach (var key in keys) {
				PlayerPrefs.DeleteKey(key);
			}
			Debug.Log("All PlayerPrefs created by this demo have been removed.");
		}
	}
}