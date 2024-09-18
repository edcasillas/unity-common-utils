using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils
{
	/// <summary>
	/// A flexible singleton registry that stores and provides singleton instances by type.
	/// This registry allows registering, resolving, and disposing of instances, and ensures
	/// that each type is registered as a single instance.
	/// </summary>
	public static class SingletonRegistry
	{
		/// <summary>
		/// Internal dictionary holding instances, keyed by their type.
		/// </summary>
		private static readonly Dictionary<Type, object> instances = new();

		/// <summary>
		/// Registers a concrete instance as a singleton.
		/// If an instance of the same type is already registered, it will be replaced.
		/// </summary>
		/// <typeparam name="T">The type of the singleton instance.</typeparam>
		/// <param name="concreteInstance">The instance to register.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided instance is null.</exception>

		public static void Register<T>(T concreteInstance) => instances[typeof(T)] = concreteInstance;

		/// <summary>
		/// Attempts to resolve and return the singleton instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the singleton to resolve.</typeparam>
		/// <param name="instance">The resolved instance, or null if it was not found.</param>
		/// <returns>True if the instance was found; otherwise, false.</returns>
		public static bool TryResolve<T>(out T instance) where T : class
		{
			var result = instances.TryGetValue(typeof(T), out var existingInstance);
			instance = existingInstance as T;
			return result;
		}

		/// <summary>
		/// Resolves a singleton instance by either returning a previously registered instance
		/// or creating a new one using the provided constructor function.
		/// </summary>
		/// <typeparam name="T">The type of the singleton to resolve.</typeparam>
		/// <param name="ctor">A constructor function used to create the instance if it does not exist.</param>
		/// <returns>The resolved or newly created instance.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the constructor is null and the instance is not registered.</exception>
		public static T Resolve<T>(Func<T> ctor) where T : class
		{
			if (TryResolve<T>(out var result)) {
				return result;
			}

			if (ctor == null) {
				throw new ArgumentNullException(nameof(ctor), $"Cannot resolve singleton of type {typeof(T).Name} because it has not been registered and no constructor is provided.");
			}

			try {
				result = ctor.Invoke();
			} catch (Exception ex) {
				Debug.LogError($"[{nameof(SingletonRegistry)}] Could not create instance of type {typeof(T).Name} because the constructor threw an exception: {ex.Message}");
				throw;
			}

			instances.Add(typeof(T), result);
			return result;
		}

		/// <summary>
		/// Clears all registered instances from the registry.
		/// </summary>
		public static void Clear() => instances.Clear();

		/// <summary>
		/// Clears all registered instances and disposes of any that implement <see cref="IDisposable"/>.
		/// </summary>
		public static void ClearAndDispose() {
			foreach (var instance in instances.Values) {
				if (instance is not IDisposable disposable) continue;
				try {
					disposable.Dispose();
				} catch (Exception ex) {
					Debug.LogError($"[{nameof(SingletonRegistry)}] Exception disposing instance of type {instance.GetType().Name}: {ex.Message}");
				}
			}

			instances.Clear();
		}
	}
}