using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils {
	public abstract class EnhancedMonoBehaviour : MonoBehaviour, IUnityComponent, IVerbosable {
		/// <summary>
		/// When <c>true</c>, writes debug messages of this component to the console.
		/// </summary>
		[Tooltip("When checked, writes debug messages of this component to the console.")]
		[SerializeField] private bool verbose;

		/// <summary>
		/// Gets a value indicating whether this object should send messages to the log console.
		/// </summary>
		public bool IsVerbose {
			get => verbose;
			protected set => verbose = value;
		}
	}
}