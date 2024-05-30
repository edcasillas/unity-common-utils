using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	public abstract class AbstractScriptableObjectDebuggableEditor<T> : AbstractDebuggableEditor<T> where T : ScriptableObject {
		internal override DebuggableComponentData InitializeComponentData()
			=> cache.Get(Subject, DebugAllPropertiesAndMethods);
	}
}