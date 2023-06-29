// TODO Linters are work in progress 
/* using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CommonUtils.Editor
{
    public class NamespaceVerifier : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report)
        {
            // Get the list of modified C# files in the local branch
            var modifiedFiles = getModifiedCSharpFiles();
			Debug.Log($"Found {modifiedFiles.Length} modified files.");

            // Collect error messages for files without a namespace
            var errorMessages = new List<string>();

            // Analyze each modified file
            foreach (var filePath in modifiedFiles)
            {
                if (isCSharpFile(filePath))
                {
                    var fileContent = File.ReadAllText(filePath);
                    if (!hasNamespace(fileContent))
                    {
                        var suggestedNamespace = generateSuggestedNamespace(filePath);
                        var errorMessage =
							$"Class {Path.GetFileNameWithoutExtension(filePath)} doesn't have a namespace. " +
							$"Suggested namespace is {suggestedNamespace}";
                        errorMessages.Add(errorMessage);
                    }
                }
            }

            // If there are error messages, throw an exception with all the accumulated messages
            if (errorMessages.Count > 0)
            {
                var errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.AppendLine("The following files don't have a namespace:");
                foreach (var errorMessage in errorMessages)
                {
                    errorMessageBuilder.AppendLine(errorMessage);
                }

                throw new BuildFailedException(errorMessageBuilder.ToString());
            }
        }

        private string[] getModifiedCSharpFiles()
        {
            // Execute Git command to get a list of modified C# files in the local branch
            var output = executeGitCommand("diff --name-only --diff-filter=ACMRTUXB `git rev-parse --abbrev-ref --symbolic-full-name @{u}`...HEAD -- '*.cs'");
            var filePaths = output.Split('\n');
            return filePaths;
        }

		/// <summary>
		/// Check if the file has a .cs extension
		/// </summary>
        private bool isCSharpFile(string filePath) => Path.GetExtension(filePath).Equals(".cs", System.StringComparison.OrdinalIgnoreCase);
		
		/// <summary>
		/// Check if the file content contains a namespace declaration 
		/// </summary>
		private bool hasNamespace(string fileContent) => fileContent.Contains("namespace");

		private string generateSuggestedNamespace(string filePath)
        {
            // Generate the suggested namespace based on the file's folder structure
            var folderPath = Path.GetDirectoryName(filePath);
            var projectName = PlayerSettings.productName;
            var relativeFolderPath = folderPath.Substring(folderPath.IndexOf("Assets/") + 7);
            var folders = relativeFolderPath.Split(Path.DirectorySeparatorChar);
            var namespacePath = string.Join(".", folders);
            return $"{projectName}.{namespacePath}";
        }

        private string executeGitCommand(string command)
        {
            // Execute a Git command and return the output
            var processInfo = new System.Diagnostics.ProcessStartInfo("git", command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

		private class BuildFailedException : System.Exception
        {
            public BuildFailedException(string message) : base(message) { }
        }
    }
}
*/