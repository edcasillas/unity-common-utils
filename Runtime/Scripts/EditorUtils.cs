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

        public static bool SaveAsJsonFile<T>(this T source, string title = null) {
            var path = UnityEditor.EditorUtility.SaveFilePanel($"Save {typeof(T)} as JSON",
                                                               "",
                                                               $"{typeof(T)}.json",
                                                               "json");
            if (!string.IsNullOrWhiteSpace(path)) {
                var jsonContents = JsonUtility.ToJson(source);
                try {
                    System.IO.File.WriteAllText(path, jsonContents);
                    Debug.Log($"{typeof(T)} was saved as JSON at {path}");
                    return true;
                } catch (System.Exception ex) {
                    Debug.LogError($"Could not save the {typeof(T)} file: {ex.Message}");
                }
            } else {
                Debug.Log("Save cancelled");
            }

            return false;
        }
    }
}