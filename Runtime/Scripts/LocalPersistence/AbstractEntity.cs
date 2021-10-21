using System;
using UnityEngine;

namespace CommonUtils.LocalPersistence {
	/// <summary>
	/// Represents an entity that can be saved and retrieved by its specified <typeparamref name="TId"/>.
	/// This is the default implementation of <see cref="IEntity{TId}"/>.
	/// </summary>
	/// <typeparam name="TId">Type of the Id this entity uses.</typeparam>
	[Serializable]
	public abstract class AbstractEntity<TId> : IEntity<TId> {
		/// <summary>
		/// Identifier for this entity.
		/// </summary>
		[SerializeField] private TId id;
		
		public TId Id { get => id; set => id = value; }
	}
}