using CommonUtils.UnityComponents;

namespace CommonUtils.ObjectPooling {
	/// <summary>
	/// Contract for game objects that can be pooled.
	/// </summary>
	public interface IObjectFromPool : IUnityComponent {
		string PoolId { get; set; } // TODO Create a hash from this Id to avoid string lookups.

		void OnInstantiatedFromPool();

		void OnReturnedToPool();
	}
}
