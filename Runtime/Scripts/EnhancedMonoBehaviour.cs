using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils {
	public abstract class EnhancedMonoBehaviour : MonoBehaviour, IUnityComponent, IVerbosable {
		/// <summary>
		/// When <c>true</c>, writes debug messages of this component to the console.
		/// </summary>
		[Tooltip("When checked, writes debug messages of this component to the console.")]
		[SerializeField] private bool verbose;

		public bool IsVerbose => verbose;
	}
}