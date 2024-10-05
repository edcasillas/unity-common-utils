using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.WebResources;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(AbstractWebResourceComponent), true)]
	public class WebResourceComponentEditor : AbstractDebuggableEditor<AbstractWebResourceComponent> { }
}
