using UnityEngine;

namespace CommonUtils.Inspector {
    /// <summary>
    /// Draws a folder selection control for the annotated string field.
    /// </summary>
    public sealed class FolderPathAttribute : PropertyAttribute {
        public readonly string Title;
        public readonly string DefaultPath;
        public readonly bool IsRelativeToProject;
        public readonly bool AllowManualEdit;

        public FolderPathAttribute(
            string title = "Select Folder",
            string defaultPath = "",
            bool isRelativeToProject = true,
            bool allowManualEdit = false) {
            Title = title;
            DefaultPath = defaultPath;
            IsRelativeToProject = isRelativeToProject;
            AllowManualEdit = allowManualEdit;
        }
    }
}
