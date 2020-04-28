using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils {
	public class TransformDepthCalculator : MonoBehaviour {
		private static TransformDepthCalculator instance;
		public static TransformDepthCalculator Instance {
			get {
				if (!instance) {
					instance = new GameObject().AddComponent<TransformDepthCalculator>();
				}

				return instance;
			}
			private set => instance = value;
		}

		private readonly Dictionary<Transform, int> cache = new Dictionary<Transform, int>();

		private void Awake() {
			if (Instance && Instance != this) {
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		private void OnDestroy() {
			if (Instance == this) Instance = null;
		}

		public int GetDepth(Transform t) {
			if (!t.parent) return 0;
			if (!cache.ContainsKey(t)) {
				cache.Add(t, GetDepth(t.parent) + 1);
			}

			return cache[t];
		}
	}
}