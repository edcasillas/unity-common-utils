using System.IO;

namespace CommonUtils.Editor
{
	public static class DirectoryUtils {
		/// <summary>
		/// Recursively copies the entire contents of <paramref name="sourceDir"/> into <paramref name="destinationDir"/>.
		/// </summary>
		/// <param name="sourceDir">Absolute or project-relative path to the source directory to copy from.</param>
		/// <param name="destinationDir">Absolute or project-relative path to the destination directory to copy into.</param>
		/// <remarks>
		/// - If <paramref name="sourceDir"/> does not exist, the method returns without doing anything.
		/// - If <paramref name="destinationDir"/> exists, it is deleted first (i.e., the copy is a clean mirror of the source).
		/// - All subdirectories and files are copied, preserving the directory structure.
		/// - Existing files at the destination are overwritten.
		/// - Throws standard <see cref="IOException"/>/<see cref="UnauthorizedAccessException"/> if the file system operations fail.
		/// </remarks>
		public static void CopyDirectory(string sourceDir, string destinationDir) {
			if (!Directory.Exists(sourceDir)) return;
			if (Directory.Exists(destinationDir)) Directory.Delete(destinationDir, true);

			// Create the root destination directory (will be populated below).
			Directory.CreateDirectory(destinationDir);

			// Create all subdirectories at the destination before copying files.
			foreach (var directory in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories)) {
				var relative = MakeRelativePath(sourceDir, directory);
				Directory.CreateDirectory(Path.Combine(destinationDir, relative));
			}

			// Copy every file, preserving directory structure.
			foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories)) {
				var relative = MakeRelativePath(sourceDir, file);
				var destinationFile = Path.Combine(destinationDir, relative);
				var destinationFolder = Path.GetDirectoryName(destinationFile);
				if (!string.IsNullOrEmpty(destinationFolder) && !Directory.Exists(destinationFolder)) {
					Directory.CreateDirectory(destinationFolder);
				}
				File.Copy(file, destinationFile, overwrite: true);
			}
		}
		/// <summary>
		/// Creates a relative path from <paramref name="basePath"/> to <paramref name="targetPath"/>.
		/// </summary>
		/// <param name="basePath">The base path from which to compute the relative segment.</param>
		/// <param name="targetPath">The target path to relativize.</param>
		/// <returns>
		/// A relative path string from <paramref name="basePath"/> to <paramref name="targetPath"/> that
		/// does not start with a directory separator. Assumes <paramref name="targetPath"/> is within <paramref name="basePath"/>.
		/// </returns>
		/// <remarks>
		/// This method performs a simple substring-based relative computation and assumes that:
		/// - <paramref name="targetPath"/> starts with <paramref name="basePath"/> (case-sensitive comparison).
		/// - <paramref name="basePath"/> refers to a directory. A trailing directory separator is ensured internally.
		/// If these assumptions are not met, the result may be incorrect or an exception may occur.
		///
		/// Examples:
		/// - Unix/macOS:
		///   basePath: "/Users/alex/Project/Assets"
		///   targetPath: "/Users/alex/Project/Assets/Textures/UI/icon.png"
		///   returns: "Textures/UI/icon.png"
		///
		/// - Windows:
		///   basePath: "C:\Projects\Game\Assets"
		///   targetPath: "C:\Projects\Game\Assets\Scripts\Main\GameManager.cs"
		///   returns: "Scripts\Main\GameManager.cs"
		///
		/// - Trailing separator normalization:
		///   basePath: "/a/b" (no trailing slash)
		///   targetPath: "/a/b/c/d.txt"
		///   returns: "c/d.txt"
		/// </remarks>
		public static string MakeRelativePath(string basePath, string targetPath) {
			if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				basePath += Path.DirectorySeparatorChar;
			}

			var relative = targetPath.Substring(basePath.Length);
			return relative.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}
	}
}
