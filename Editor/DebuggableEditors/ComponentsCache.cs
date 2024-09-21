using CommonUtils.DebuggableEditors;
using CommonUtils.Heaps;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	internal class ComponentsCache {
		private readonly Dictionary<Object, DebuggableComponentData> cache = new();

		private readonly DynamicPriorityQueue<DebuggableComponentData> priorityQueue = new();

		private readonly int capacity;

		public int Count => priorityQueue.Count;

		public ComponentsCache(int capacity) => this.capacity = capacity;

		public DebuggableComponentData Get<T>(T subject, bool debugAllPropertiesAndMethods = false, bool debugAllMonoBehaviorPropertiesAndMethods = false) where T : Object {
			if (!cache.TryGetValue(subject, out var result)) {
				if (priorityQueue.Count >= capacity) {
					removeLeastUsed();
				}
				result = createData(subject, debugAllPropertiesAndMethods, debugAllMonoBehaviorPropertiesAndMethods);
				cache.Add(subject, result);
			}

			result.Timestamp = Time.realtimeSinceStartup;
			priorityQueue.Enqueue(result);
			return result;
		}

		private DebuggableComponentData createData<T>(T subject, bool debugAllPropertiesAndMethods, bool debugAllMonoBehaviorPropertiesAndMethods) {
			return new DebuggableComponentData() {
				DebuggableProperties = subject.GetType()
					.GetDebuggableProperties(debugAllPropertiesAndMethods,
						debugAllMonoBehaviorPropertiesAndMethods),
				DebuggableMethods = subject.GetType()
					.GetDebuggableMethods(debugAllPropertiesAndMethods, debugAllMonoBehaviorPropertiesAndMethods)
			};
		}

		private void removeLeastUsed() {
			var leastUsed = priorityQueue.Dequeue();
			cache.Remove(leastUsed.Subject);
		}
	}
}