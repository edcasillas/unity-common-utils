namespace CommonUtils.ObjectPooling {
	public static class ObjectFromPoolExtensions {
		/// <summary>
		/// "Destroys" this object and returns it to the pool.
		/// </summary>
		public static void Recycle(this IObjectFromPool objectFromPool)
			=> PrefabPoolManager.DestroyAndReturnToPool(objectFromPool.gameObject);

		/// <summary>
		/// Returns <c>true</c> when the object is in the pool, that is, it should not be considered a valid object
		/// even though it might be not null and not destroyed.
		/// </summary>
		public static bool IsInPool(this IObjectFromPool objectFromPool) => PrefabPoolManager.Contains(objectFromPool);
	}
}