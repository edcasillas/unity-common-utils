using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.UI;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(SimpleColorPicker))]
	public class SimpleColorPickerEditor : AbstractDebuggableEditor<SimpleColorPicker> { }
}