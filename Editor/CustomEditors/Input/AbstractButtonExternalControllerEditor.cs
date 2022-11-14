using CommonUtils.Editor.DebuggableEditors;
using System.Linq;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;

namespace CommonUtils.Editor.CustomEditors.Input {
    [CustomEditor(typeof(AbstractButtonExternalController), true)]
    public class AbstractButtonExternalControllerEditor : AbstractDebuggableEditor<AbstractButtonExternalController> {
        private bool showCurrentBlockers;

        protected override void RenderDebug() {
			base.RenderDebug();
			EditorGUILayout.Space();
            showCurrentBlockers = EditorExtensions.ReadonlyEnumerable(showCurrentBlockers,
                Subject.CurrentlyBlockedBy.Select(b => b.gameObject), "Currently Blocked By");
        }
    }
}