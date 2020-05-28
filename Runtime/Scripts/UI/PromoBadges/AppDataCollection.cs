using System;
using CommonUtils.Inspector.ReorderableInspector;

namespace CommonUtils.UI.PromoBadges {
	[Serializable]
	internal class AppDataCollection {
		[Reorderable] public AppData[] Apps;
	}
}