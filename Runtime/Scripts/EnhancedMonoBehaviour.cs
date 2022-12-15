using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils {
	public abstract class EnhancedMonoBehaviour : MonoBehaviour, IUnityComponent, IVerbosable {
		[SerializeField] private bool verbose;

		public bool IsVerbose => verbose;
	}
}