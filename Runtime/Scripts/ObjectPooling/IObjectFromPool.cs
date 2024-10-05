using CommonUtils.UnityComponents;

namespace CommonUtils.ObjectPooling {
	/// <summary>
	/// Contract for game objects that can be pooled.
	/// </summary>
	public interface IObjectFromPool : IUnityComponent {
		/// <summary>
		/// Gets or sets the ID of the pool to which this object belongs.
		/// </summary>
		/// <remarks>
		/// This property is set by <see cref="PrefabPool"/>. Implementors should not worry about implementing this
		/// property and should only repeat the declaration in the interface.
		/// </remarks>
		[ShowInInspector]
		string PoolId { get; set; } // TODO Create a hash from this Id to avoid string lookups.

		void OnInstantiatedFromPool();

		void OnReturnedToPool();
	}
}
