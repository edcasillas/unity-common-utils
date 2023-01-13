namespace CommonUtils.ObjectPooling {
	/// <summary>
	/// This component provides a basic implementation of <see cref="IObjectFromPool"/> that can be used by most
	/// games in order to make objects poolable.
	///
	/// There are cases in which this component cannot be used, such as games using Photon, in which case networked
	/// components must implement Photon.PunBehaviour; if this is your case, just make sure your PunBehaviour implements
	/// the IObjectFromPool interface.
	/// </summary>
	public class BasicObjectFromPool : EnhancedMonoBehaviour, IObjectFromPool {
		public string PoolId { get; set; }
		public virtual void OnInstantiatedFromPool() { }
		public virtual void OnReturnedToPool() { }
	}
}