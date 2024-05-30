using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	/// <summary>
    /// Base class for custom editors that show two sections while playing in the editor:
    /// - "Configuration" shows the default inspector to set up the target <see cref="Subject"/>,
    /// or whatever modifications are done to this default inspector by overriding <see cref="RenderConfig"/>.
    /// - "Debug" shows values that only make sense during play mode. Inheritors must override <see cref="RenderDebug"/>.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="MonoBehaviour"/> for which this class will be a custom editor.</typeparam>
    public abstract class AbstractMonoBehaviourDebuggableEditor<T> : AbstractDebuggableEditor<T> where T : MonoBehaviour {
		/// <summary>
		/// When set to <c>true</c> and <see cref="DebugAllPropertiesAndMethods"/> is also <c>true</c>, the debuggable
		/// editor will include public properties and methods from <see cref="MonoBehaviour"/> and base classes.
		/// </summary>
		protected virtual bool DebugAllMonoBehaviorPropertiesAndMethods => false;

		internal override DebuggableComponentData InitializeComponentData() => cache.Get(Subject, DebugAllPropertiesAndMethods, DebugAllMonoBehaviorPropertiesAndMethods);

		internal override bool ShouldOnlyShowConfig() => base.ShouldOnlyShowConfig() || Subject.gameObject.scene.rootCount == 0;
    }
}