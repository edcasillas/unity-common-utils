using CommonUtils.Editor.DebuggableEditors;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(EnhancedMonoBehaviour), true)]
	public class EnhancedMonoBehaviourEditor : AbstractMonoBehaviourDebuggableEditor<EnhancedMonoBehaviour> { }
}