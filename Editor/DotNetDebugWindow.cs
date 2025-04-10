using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace CommonUtils {
	public class DotNetDebugWindow : EditorWindow {
		[MenuItem("Tools/Debug .NET Path...")]
		public static void ShowDotnetInfo() {try
			{
				var process = new Process();
				process.StartInfo.FileName = "which";
				process.StartInfo.Arguments = "dotnet";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();

				var output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				if (string.IsNullOrWhiteSpace(output)) {
					Debug.LogWarning(".NET Path could not be found.");
				} else {
					Debug.Log($"Dotnet path in Unity: {output}");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error running 'which dotnet': {ex}");
			}
		}
	}
}