namespace CommonUtils.ObjectPooling {
	public static class ObjectFromPoolExtensions {
		/// <summary>
		/// "Destroys" this object and returns it to the pool.
		/// </summary>
		public static void DestroyMe(this IObjectFromPool objectFromPool)
			=> PrefabPoolManager.DestroyAndReturnToPool(objectFromPool.gameObject);
	}
}