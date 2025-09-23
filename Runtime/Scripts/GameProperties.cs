using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils {
	[Serializable]
	public class GamePropertyEntry {
		[SerializeField] private string key;
		[SerializeField] private string value;

		public string Key => key;
		public string Value => value;
	}

	[CreateAssetMenu(menuName = "Common Utils/Game Properties")]
	public class GameProperties : ScriptableObject {
		[SerializeField] private GamePropertyEntry[] entries;

		private Dictionary<string, string> propertiesDict;

		public IReadOnlyDictionary<string, string> Properties {
			get {
				if (propertiesDict == null) Reload();
				return propertiesDict;
			}
		}

		public string this[string key] => Properties.GetValueOrDefault(key);

		public void Reload() => propertiesDict = entries?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, string>();

		public static GameProperties Load(string assetName) => Resources.Load<GameProperties>(assetName);
	}
}