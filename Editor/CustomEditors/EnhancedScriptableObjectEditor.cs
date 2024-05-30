using CommonUtils.Editor.DebuggableEditors;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(EnhancedScriptableObject), true)]
	public class EnhancedScriptableObjectEditor : AbstractScriptableObjectDebuggableEditor<EnhancedScriptableObject> { }
}