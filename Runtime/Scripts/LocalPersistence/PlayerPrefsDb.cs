using System;
using UnityEngine;

namespace CommonUtils.LocalPersistence {
	/// <summary>
	/// Extends Unity's built-in PlayerPrefs to make it act as a very simple, json-based database.
	/// </summary>
	public static class PlayerPrefsDb {
		/// <summary>
		/// Saves the <paramref name="entity"/> in local storage.
		/// </summary>
		/// <typeparam name="TEntity">Type of entity to save.</typeparam>
		/// <typeparam name="TId">Type of Id of the entity to save.</typeparam>
		/// <param name="entity">Entity to save.</param>
		/// <param name="customId">Custom identifier to set for this entity in the local storage.</param>
		public static void SaveLocal<TEntity, TId>(this TEntity entity, string customId = null) where TEntity : class, IEntity<TId> {
			if (entity == default(TEntity)) throw new ArgumentNullException(nameof(entity));
			var saveKey   = getSaveKey<TEntity, TId>(entity, customId);
			var saveValue = JsonUtility.ToJson(entity);
			PlayerPrefs.SetString(saveKey, saveValue);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Retrieves an entity of type <typeparamref name="TEntity"/> from local storage, searching by its specified <paramref name="id"/>.
		/// </summary>
		/// <typeparam name="TEntity">Type of entity to retrieve.</typeparam>
		/// <typeparam name="TId">Type of Id of the entity to retrieve.</typeparam>
		/// <param name="id">Id of the entity to retrieve.</param>
		/// <param name="customId">Custom identifier to set for this entity in the local storage.</param>
		/// <returns>Entity of type <typeparamref name="TEntity"/> with the specified <paramref name="id"/> that was previously saved into PlayerPrefs, or null if it doesn't exist.</returns>
		public static TEntity RetrieveFromLocal<TEntity, TId>(TId id, string customId = null) where TEntity : IEntity<TId> {
			var result     = default(TEntity);
			var     saveKey    = getSaveKey<TEntity, TId>(id, customId);
			var     savedValue = PlayerPrefs.GetString(saveKey, null);
			if (savedValue == null) return default;
			try {
				result = JsonUtility.FromJson<TEntity>(savedValue);
			} catch (Exception ex) {
				Debug.LogError($"A value for entity \"{saveKey}\" was found in local storage, but it couldn't be deserialized: {ex.Message}");
			}

			return result;
		}

		#region Private methods
		/// <summary>
		/// Builds a key to save the specified <paramref name="entity"/> in PlayerPrefs.
		/// </summary>
		/// <typeparam name="TEntity">Type of the entity.</typeparam>
		/// <typeparam name="TId">Type of Id of the entity.</typeparam>
		/// <param name="entity">Entity to get its save key.</param>
		/// <param name="customSuffix">Custom suffix to add to the key.</param>
		/// <returns>Save key for PlayerPrefs of the specified <paramref name="entity"/>.</returns>
		private static string getSaveKey<TEntity, TId>(TEntity entity, string customSuffix = null) where TEntity : IEntity<TId> => getSaveKey<TEntity, TId>(entity.Id, customSuffix);

		/// <summary>
		/// Builds a key to save an entity of type <typeparamref name="TEntity"/> in PlayerPrefs.
		/// </summary>
		/// <typeparam name="TEntity">Type of the entity.</typeparam>
		/// <typeparam name="TId">Type of Id of the entity.</typeparam>
		/// <param name="id">Entity to get its save key.</param>
		/// <param name="customSuffix">Custom suffix to add to the key.</param>
		/// <returns>Save key for PlayerPrefs of the specified <paramref name="id"/></returns>
		private static string getSaveKey<TEntity, TId>(TId id, string customSuffix = null) where TEntity : IEntity<TId> => $"{typeof(TEntity).Name}{id.ToString()}{(!string.IsNullOrEmpty(customSuffix) ? $"-{customSuffix}" : string.Empty)}";
		#endregion
	}
}