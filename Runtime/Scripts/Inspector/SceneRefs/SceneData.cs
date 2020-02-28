using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Inspector.SceneRefs {
    [Serializable]
    public class SceneData {
        public int    buildIndex;
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

            for (var index = 0; index < UnityEditor.EditorBuildSettings.scenes.Length; ++index) {
                if (assetGUID.Equals(UnityEditor.EditorBuildSettings.scenes[index].guid)) {
                    entry.buildIndex = index;
                    return entry;
                }
            }

            return entry;
            #else
            Debug.LogError($"{nameof(SceneData)}.{nameof(GetFrom)} is an Editor only method. Returning null");
            return null;
            #endif
        }
    }
}