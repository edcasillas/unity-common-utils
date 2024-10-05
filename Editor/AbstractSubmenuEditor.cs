using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.UI.Submenus;
using UnityEditor;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(AbstractSubmenu), true)]
	public class AbstractSubmenuEditor : AbstractDebuggableEditor<AbstractSubmenu> { }
}