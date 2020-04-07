using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class RendererExtensions {
		public static Bounds GetBounds(this IEnumerable<Renderer> renderers) {
			if (renderers.IsNullOrEmpty()) return new Bounds();

			var result = new Bounds {center = renderers.First().transform.position};
			foreach (var renderer in renderers) {
				result.Encapsulate(renderer.bounds);
			}

			return result;
		}

		public static BoundingSphere GetBoundingSphere(this IEnumerable<Renderer> renderers) {
			var bounds = renderers.GetBounds();
			return new BoundingSphere {position = bounds.center, radius = bounds.extents.magnitude};
		}
	}
}