using UnityEngine;

namespace CommonUtils.Editor.ScreenshotManager {
    [CreateAssetMenu(fileName = "ScreenshotManagerConfig", menuName = "Screenshot Manager/Configuration")]
    public class ScreenshotManagerConfig : ScriptableObject {
        public string  SaveToFolder = "Screenshots";
        public string  FilePrefix   = "screenshot";
        public int CurrentCount = 0;
    }
}