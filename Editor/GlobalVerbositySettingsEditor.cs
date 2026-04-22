using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CommonUtils.Editor
{
    public class GlobalVerbositySettingsEditor : EditorWindow
    {
        private bool useGlobalVerbosity = false;
        private string[] verbosityLevels = { "None", "Debug", "Warning", "Error" };
        private int selectedVerbosityIndex = 2; // Default to "Warning"
        private string currentDefineSymbol = "";

        [MenuItem(MenuItems.MenuPathPrefix + "Global Verbosity Settings...", priority = 2)]
        public static void ShowWindow() => GetWindow<GlobalVerbositySettingsEditor>("Global Verbosity Settings");

        private void OnEnable() => LoadCurrentDefineSymbol();

		private void OnGUI()
        {
            useGlobalVerbosity = EditorGUILayout.Toggle("Use global verbosity level", useGlobalVerbosity);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(new GUIContent("Selected build target group", "Change this target group in the build settings."),
                EditorUserBuildSettings.selectedBuildTargetGroup);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!useGlobalVerbosity);
            selectedVerbosityIndex = EditorGUILayout.Popup("Global Verbosity Level", selectedVerbosityIndex, verbosityLevels);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Apply"))
            {
                ApplyVerbosityLevel();
            }
        }

        private void ApplyVerbosityLevel()
        {
			var symbols = getScriptingDefineSymbols();

            // Remove the current define symbol if it exists
            if (!string.IsNullOrEmpty(currentDefineSymbol))
            {
                symbols = symbols.Replace(currentDefineSymbol, "").Replace(";;", ";").Trim(';');
            }

            if (useGlobalVerbosity)
            {
                // Add the new define symbol
                currentDefineSymbol = "GLOBAL_VERBOSITY_" + verbosityLevels[selectedVerbosityIndex].ToUpper();
                if (!symbols.Contains(currentDefineSymbol))
                {
                    symbols = symbols + ";" + currentDefineSymbol;
                }
            }
            else
            {
                // Clear the current define symbol if not using global verbosity
                currentDefineSymbol = "";
            }

			setScriptingDefineSymbols(symbols);
        }

        private void LoadCurrentDefineSymbol()
        {
			var symbols = getScriptingDefineSymbols();

            useGlobalVerbosity = false;
            selectedVerbosityIndex = 2; // Default to "Warning"

            foreach (var verbosity in verbosityLevels)
            {
                var defineSymbol = "GLOBAL_VERBOSITY_" + verbosity.ToUpper();
                if (symbols.Contains(defineSymbol))
                {
                    useGlobalVerbosity = true;
                    currentDefineSymbol = defineSymbol;
                    selectedVerbosityIndex = System.Array.IndexOf(verbosityLevels, verbosity);
                    break;
                }
            }
        }

		private string getScriptingDefineSymbols() {
#if UNITY_6000
			return PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
#else
			return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
		}

		private void setScriptingDefineSymbols(string symbols) {
#if UNITY_6000
			PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), symbols);
#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
#endif
		}
    }
}
