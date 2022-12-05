namespace CommonUtils {
	/// <summary>
	/// This utility makes it easy to determine when an assembly reload is occurring in the Unity editor.
	/// Use it to gate access to functionality inside implementations of ISerializationCallbackReceiver.OnBeforeSerialize
	/// to avoid errors with the message:
	/// "Objects are trying to be loaded during a domain backup. This is not allowed as it will lead to undefined behaviour!"
	/// </summary>
	/// <example>
	/// <code>
	/// public void OnBeforeSerialize() {
	///		if(AssemblyReloadUtil.IsReloading) return; // This fixes the error.
	///		// Your code here.
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// The error "Objects are trying to be loaded during a domain backup. This is not allowed as it will lead to undefined behaviour!"
	/// was found happening when trying to call <c>Resources.Load</c> during OnBeforeSerialize; adding <c>if(AssemblyReloadUtil.IsReloading)</c>
	/// before the offending call fixes the issue.
	///
	/// This solution is based off this suggestion: https://forum.unity.com/threads/objects-are-trying-to-be-loaded-during-a-domain-backup.1264967/#post-8058242
	/// </remarks>
	#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	#endif
	public static class AssemblyReloadUtil {
		public static bool IsReloading { get; private set; }

		#if UNITY_EDITOR
		static AssemblyReloadUtil() {
			UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += onBeforeAssemblyReload;
			UnityEditor.AssemblyReloadEvents.afterAssemblyReload += onAfterAssemblyReload;
		}
		#endif

		private static void onAfterAssemblyReload() => IsReloading = false;

		private static void onBeforeAssemblyReload() => IsReloading = true;
	}
}