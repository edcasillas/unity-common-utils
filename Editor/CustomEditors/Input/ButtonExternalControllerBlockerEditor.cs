using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors.Input {
    [CustomEditor(typeof(ButtonExternalControllerBlocker))]
    public class ButtonExternalControllerBlockerEditor : AbstractDebuggableEditor<ButtonExternalControllerBlocker> { }
}