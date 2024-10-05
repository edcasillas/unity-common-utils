using System;
using Object = UnityEngine.Object;

namespace CommonUtils.Inspector.SceneRefs {
	[Serializable]
	public class SceneData {
		public int buildIndex;
		public string assetPath;

		/// <summary>
		/// For a given Scene Asset object reference, extract its build settings data, including buildIndex.
		/// </summary>
		public static SceneData GetFrom(Object sceneObject) {
			#if UNITY_EDITOR
			var entry = new SceneData {
				buildIndex = -1
			};

			if (sceneObject as UnityEditor.SceneAsset == null)
				return null;

			entry.assetPath = UnityEditor.AssetDatabase.GetAssetPath(sceneObject);
			var assetGUID = new UnityEditor.GUID(UnityEditor.AssetDatabase.AssetPathToGUID(entry.assetPath));

			int actualValidIndex = 0;
			for (var index = 0; index < UnityEditor.EditorBuildSettings.scenes.Length; ++index) {
				if (assetGUID.Equals(UnityEditor.EditorBuildSettings.scenes[index].guid)) {
					entry.buildIndex = actualValidIndex;
					return entry;
				}
				actualValidIndex += UnityEditor.EditorBuildSettings.scenes[index].enabled ? 1 : 0;
			}

			return entry;
			#else
            // SceneData.GetFrom is an Editor only method and should not be used in any other context.
			return null;
			#endif
		}
	}
}