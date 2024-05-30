using CommonUtils.Editor.DebuggableEditors;
using System.Linq;
using CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation;
using CommonUtils.UnityComponents;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SequentialKeyboardNavigationManager))]
    public class SequentialKeyboardNavigationManagerEditor : AbstractMonoBehaviourDebuggableEditor<SequentialKeyboardNavigationManager> { }
}