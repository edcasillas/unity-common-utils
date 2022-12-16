using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.UI.Submenus.Toasty {
	public class ToastManager : EnhancedMonoBehaviour {
		[SerializeField] private Toast toastPrefab;

		[ShowInInspector] public Queue<Toast> Pool { get; } = new Queue<Toast>();

		[ShowInInspector]
		public void Show(string message, Sprite sprite) {
			var toast = getToast();
			toast.ChangeDisplayedValues(message, sprite);
			toast.Show();
		}

		private Toast getToast() {
			if (!Pool.Any()) {
				var result = Instantiate(toastPrefab, transform);
				result.SubscribeOnHidden(() => Pool.Enqueue(result));
				return result;
			}

			return Pool.Dequeue();
		}
	}
}