using CommonUtils.Editor.DebuggableEditors;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(DebuggableMonoBehaviour), true)]
	public class DebuggableMonoBehaviourEditor : AbstractDebuggableEditor<DebuggableMonoBehaviour> { }
}