using CommonUtils.Editor.DebuggableEditors;
using System.Linq;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors.Input {
    [CustomEditor(typeof(ButtonExternalControllerBlocker))]
    public class ButtonExternalControllerBlockerEditor : AbstractDebuggableEditor<ButtonExternalControllerBlocker> {
        private bool showSubscribers = true;
        
        protected override void RenderDebug() {
            showSubscribers = EditorExtensions.ReadonlyEnumerable(showSubscribers,
                Subject.Subscribers.Select(s => s.gameObject), "Subscribers");
        }
    }
}