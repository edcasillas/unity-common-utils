using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils
{
	public static class ServiceLocator
	{
		private static readonly Dictionary<Type, object> singletons = new();
		private static readonly Dictionary<Type, Func<object>> factories = new();

		public static void RegisterSingleton<TDependency>(TDependency concreteInstance) => singletons[typeof(TDependency)] = concreteInstance;
		public static void RegisterFactory<T>(Func<T> factory) => factories[typeof(T)] = () => factory();

		public static TDependency Resolve<TDependency>(Func<TDependency> ctor) where TDependency : class
		{
			if (singletons.TryGetValue(typeof(TDependency), out var existingInstance))
			{
				return existingInstance as TDependency;
			}

			if (ctor == null)
			{
				Debug.LogError($"Cannot resolve dependency of type {typeof(TDependency).Name} because constructor is not provided.");
				return null;
			}

			var result = ctor.Invoke();
			singletons.Add(typeof(TDependency), result);
			Debug.Log($"[Tests] {typeof(TDependency).Name} resolved as {result.GetType().Name}");
			return result;
		}

		public static bool TryResolve<TDependency>(out TDependency instance) where TDependency : class
		{
			var result = singletons.TryGetValue(typeof(TDependency), out var existingInstance);
			instance = existingInstance as TDependency;
			return result;
		}

		public static void Clear() => singletons.Clear();
	}
}