using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	[CustomEditor(typeof(DebuggableProxy))]
	public class DebuggableProxyEditor : UnityEditor.Editor {
		private static readonly ComponentsCache cache = new ComponentsCache(100);

		private DebuggableComponentData componentData;

		protected DebuggableProxy Subject { get; private set; }

		private void OnEnable() {
			if(!target) return;
			Subject = (DebuggableProxy)target;

			Subject.OnTargetChanged += refreshComponentData;

			refreshComponentData();
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			if (!Subject.Target || !Application.isPlaying || Subject.gameObject.scene.rootCount == 0) {
				return;
			}

			componentData.ShowDebug = EditorExtensions.Collapse(componentData.ShowDebug, $"Debug {Subject.Target.GetType().Name}", ()=> componentData.RenderDebuggableMembers(Subject.Target));
			EditorUtility.SetDirty(target);
		}

		private void refreshComponentData() {
			if (!Subject.Target) {
				componentData = null;
				return;
			}
			componentData = cache.Get(Subject.Target, true);
		}
	}
}
