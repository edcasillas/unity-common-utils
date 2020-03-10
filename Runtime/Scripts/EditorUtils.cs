using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils {
    public static class EditorUtils {
        public static void HighlightAssetOfType<TAsset>(string assetPath) where TAsset : Object {
            #if UNITY_EDITOR
            var instance = (TAsset) Resources.Load(assetPath, typeof(TAsset));

            if (!instance) {
                Debug.LogError($"Asset of type {typeof(TAsset)} could not be found at \"{assetPath}\"");
                return;
            }

            UnityEditor.Selection.objects = new Object[] {instance};
            UnityEditor.EditorGUIUtility.PingObject(instance);
            #endif
        }
    }
}