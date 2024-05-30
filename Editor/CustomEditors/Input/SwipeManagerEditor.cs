using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.Input;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SwipeManager))]
    public class SwipeManagerEditor : AbstractMonoBehaviourDebuggableEditor<SwipeManager> { }
}