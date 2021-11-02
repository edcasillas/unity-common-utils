using UnityEngine;

namespace CommonUtils.Editor {
    public static class ConsoleUtils {
        /// <summary>
        /// Clears all log entries from the Console view.
        /// </summary>
        public static void Clear() {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            if (logEntries == null) {
                Debug.LogError("An error occured while trying to clear the console: Could not find the type LogEntries in UnityEditor.dll");
                return;
            }

            var clearMethod = logEntries.GetMethod("Clear",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (clearMethod == null) {
                Debug.LogError("An error occured while trying to clear the console: The type LogEntries in UnityEditor.dll does not have a Clear method.");
                return;
            }

            clearMethod.Invoke(null, null);
        }
    }
}
