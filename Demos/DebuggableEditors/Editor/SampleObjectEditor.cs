using CommonUtils.Editor.DebuggableEditors;
using UnityEditor;

namespace Demos.DebuggableEditors {
	[CustomEditor(typeof(SampleObject))]
	public class SampleObjectEditor : AbstractDebuggableEditor<SampleObject> { }
}