using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace CommonUtils.Editor.Linters {
	[InitializeOnLoad]
	public class Linter {
		static Linter() => CompilationPipeline.compilationStarted += onCompilationStarted;

		private static void onCompilationStarted(object obj) {
			// Get the list of modified C# files in the local branch
			var modifiedFiles = getModifiedCSharpFiles();
			Debug.Log($"Will analyze {modifiedFiles.Length} files: {string.Join(",", modifiedFiles)}");
		}

		private static string[] getModifiedCSharpFiles() {
			var modifiedFiles = new List<string>();

			// Execute Git command to get a list of modified C# files in the local branch
			var processInfo = new System.Diagnostics.ProcessStartInfo("git", "diff --name-only") {
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			var process = new System.Diagnostics.Process();
			process.StartInfo = processInfo;
			process.OutputDataReceived += (sender, e) => {
				if (!string.IsNullOrEmpty(e.Data)) {
					modifiedFiles.Add(e.Data);
				}
			};

			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();

			return modifiedFiles.ToArray();
		}
	}

	public static class LintUtils {
		/// <summary>
		/// Check if the file has a .cs extension
		/// </summary>
		public static bool IsPathOfCSharpFile(this string filePath) => Path.GetExtension(filePath).Equals(".cs", System.StringComparison.OrdinalIgnoreCase);
		
		/// <summary>
		/// Generate the suggested namespace based on the file's folder structure
		/// </summary>
		public static string GetSuggestedNamespace(this string filePath) {
			// Generate the suggested namespace based on the file's folder structure
			var folderPath = Path.GetDirectoryName(filePath);
			var projectName = PlayerSettings.productName;
			var relativeFolderPath = folderPath.Substring(folderPath.IndexOf("Assets/") + 7);
			var folders = relativeFolderPath.Split(Path.DirectorySeparatorChar);
			var namespacePath = string.Join(".", folders);
			return $"{projectName}.{namespacePath}";
		}
	}

	public class NamespaceVerifier : AssetPostprocessor {
		private static void OnGeneratedCSProjectFiles() => verifyNamespaces();

		private static void verifyNamespaces() {
			// Get the list of modified C# files in the local branch
			string[] modifiedFiles = getModifiedCSharpFiles();

			// Collect error messages for files without a namespace
			List<string> errorMessages = new List<string>();

			// Analyze each modified file
			foreach (string filePath in modifiedFiles) {
				if (filePath.IsPathOfCSharpFile()) {
					string fileContent = File.ReadAllText(filePath);
					if (!fileContent.Contains("namespace")) {
						string suggestedNamespace = filePath.GetSuggestedNamespace();
						string errorMessage =
							string.Format("Class {0} doesn't have a namespace. Suggested namespace is {1}",
								Path.GetFileNameWithoutExtension(filePath), suggestedNamespace);
						errorMessages.Add(errorMessage);
					}
				}
			}

			// If there are error messages, log them as warnings
			if (errorMessages.Count > 0) {
				var errorMessageBuilder = new StringBuilder();
				errorMessageBuilder.AppendLine("The following files don't have a namespace:");
				foreach (var errorMessage in errorMessages) {
					errorMessageBuilder.AppendLine(errorMessage);
				}

				Debug.LogWarning(errorMessageBuilder.ToString());
			}
		}

		private static string[] getModifiedCSharpFiles() {
			string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
			return files;
		}
	}
}