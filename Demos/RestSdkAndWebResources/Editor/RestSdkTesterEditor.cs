using CommonUtils.Editor.DebuggableEditors;
using UnityEditor;

namespace Demos.RestSdkAndWebResources.Editor {
	[CustomEditor(typeof(RestSdkTester))]
	public class RestSdkTesterEditor : AbstractDebuggableEditor<RestSdkTester> { }
}
