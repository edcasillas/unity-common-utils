using CommonUtils.Inspector.ReorderableInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.DynamicEnums {
	[CreateAssetMenu(menuName = "Common Utils/Dynamic Enum Definitions")]
	public class DynamicEnumDefinitions : ScriptableObject {
		#pragma warning disable 649
		[SerializeField] [Reorderable] private DynamicEnum[] enums;
		#pragma warning restore 649

		private Dictionary<string, DynamicEnum> htEnums;

		public IReadOnlyDictionary<string, DynamicEnum> Enums {
			get {
				if(htEnums == null) Reload();
				return htEnums;
			}
		}

		public DynamicEnum this[string enumName] => Enums.TryGetValue(enumName, out var result) ? result : null;

		public void Reload() => htEnums = enums.ToDictionary(e => e.Name, e => e);
	}
}
