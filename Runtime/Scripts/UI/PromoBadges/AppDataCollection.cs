using CommonUtils.Inspector.ReorderableInspector;
using UnityEngine;

namespace CommonUtils.UI.PromoBadges {
	[CreateAssetMenu(menuName = "Common Utils/UI/Promo Badge App Data Collection", fileName = "PromoBadgeApps")]
	internal class AppDataCollection : ScriptableObject {
		[Reorderable] public AppData[] Apps;
	}
}