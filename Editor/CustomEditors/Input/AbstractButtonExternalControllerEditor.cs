using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors.Input {
    [CustomEditor(typeof(AbstractButtonExternalController), true)]
    public class AbstractButtonExternalControllerEditor : AbstractDebuggableEditor<AbstractButtonExternalController> { }
}