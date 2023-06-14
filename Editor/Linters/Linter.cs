using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace CommonUtils.Editor.Linters
{
	[InitializeOnLoad]
    public class Linter : AssetPostprocessor
    {
		static Linter() {
			CompilationPipeline.compilationStarted += onCompilationStarted;
		}

		private static void onCompilationStarted(object obj) {
			// Get the list of modified C# files in the local branch
			var modifiedFiles = getModifiedCSharpFiles();

			Debug.Log($"Will analyze {modifiedFiles.Length} files: {string.Join(",", modifiedFiles)}");
		}
		
		private static string[] getAllFilesInProject() => Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
		
		/// <summary>
		/// Execute Git command to get a list of modified C# files in the local branch
		/// </summary>
		private static string[] getModifiedCSharpFiles()
		{
			var output = ExecuteGitCommand("diff --name-only --diff-filter=ACMRTUXB `git rev-parse --abbrev-ref --symbolic-full-name @{u}`...HEAD -- '*.cs'");
			Debug.Log($"Git got '{output}'");
			var filePaths = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			return filePaths;
		}
		
		private static string ExecuteGitCommand(string command)
		{
			// Execute a Git command and return the output
			var processInfo = new System.Diagnostics.ProcessStartInfo("git", command)
			{
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			var process = System.Diagnostics.Process.Start(processInfo);
			string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return output;
		}
	}
	
	public class NamespaceVerifier : AssetPostprocessor
    {
		
		
        private static void OnGeneratedCSProjectFiles()
        {
            VerifyNamespaces();
        }

        private static void VerifyNamespaces()
        {
			// Get the list of modified C# files in the local branch
			string[] modifiedFiles = GetModifiedCSharpFiles();
			
			// Collect error messages for files without a namespace
            List<string> errorMessages = new List<string>();

            // Analyze each modified file
            foreach (string filePath in modifiedFiles)
            {
                if (IsCSharpFile(filePath))
                {
                    string fileContent = File.ReadAllText(filePath);
                    if (!HasNamespace(fileContent))
                    {
                        string suggestedNamespace = GenerateSuggestedNamespace(filePath);
                        string errorMessage = string.Format("Class {0} doesn't have a namespace. Suggested namespace is {1}", Path.GetFileNameWithoutExtension(filePath), suggestedNamespace);
                        errorMessages.Add(errorMessage);
                    }
                }
            }

            // If there are error messages, log them as warnings
            if (errorMessages.Count > 0)
            {
                StringBuilder errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.AppendLine("The following files don't have a namespace:");
                foreach (string errorMessage in errorMessages)
                {
                    errorMessageBuilder.AppendLine(errorMessage);
                }

                Debug.LogWarning(errorMessageBuilder.ToString());
            }
        }

        private static string[] GetModifiedCSharpFiles()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            return files;
        }

        private static bool IsCSharpFile(string filePath)
        {
            // Check if the file has a .cs extension
            return Path.GetExtension(filePath).Equals(".cs", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasNamespace(string fileContent)
        {
            // Check if the file content contains a namespace declaration
            return fileContent.Contains("namespace");
        }

        private static string GenerateSuggestedNamespace(string filePath)
        {
            // Generate the suggested namespace based on the file's folder structure
            string folderPath = Path.GetDirectoryName(filePath);
            string projectName = PlayerSettings.productName;
            string relativeFolderPath = folderPath.Substring(folderPath.IndexOf("Assets/") + 7);
            string[] folders = relativeFolderPath.Split(Path.DirectorySeparatorChar);
            string namespacePath = string.Join(".", folders);
            return string.Format("{0}.{1}", projectName, namespacePath);
        }
    }
}
