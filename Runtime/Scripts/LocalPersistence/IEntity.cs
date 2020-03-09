namespace CommonUtils.LocalPersistence {
	/// <summary>
	/// Represents an entity that can be saved and retrieved by its specified <typeparamref name="TId"/>.
	/// </summary>
	/// <typeparam name="TId">Type of the Id this entity uses.</typeparam>
	public interface IEntity<TId> {
		/// <summary>
		/// Identifier for this entity.
		/// </summary>
		TId Id { get; set; }
	}
}
