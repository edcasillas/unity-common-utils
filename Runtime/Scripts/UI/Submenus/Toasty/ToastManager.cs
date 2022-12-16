using CommonUtils.Inspector.HelpBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.UI.Submenus.Toasty {
	public class ToastManager : EnhancedMonoBehaviour {
		private class ToastConfig {
			public string Message;
			public Sprite Sprite;
		}

		[SerializeField] private Toast toastPrefab;

		[Space]
		[HelpBox("Optional but recommended. If maxShown is greater than zero, the amount of toasts that can be shown at the same time in the screen will be limited. ")]
		[SerializeField] private int maxShown = 0;

		[ShowInInspector] public Queue<Toast> Pool { get; } = new Queue<Toast>();

		[ShowInInspector] public int ActiveToastsCount { get; private set; }

		private readonly Queue<ToastConfig> pendingToasts = new Queue<ToastConfig>();
		[ShowInInspector] public int PendingToastsCount => pendingToasts.Count;

		[ShowInInspector]
		public void Show(string message, Sprite sprite) {
			if (maxShown > 0 && ActiveToastsCount >= maxShown) {
				return;
			}

			var toast = getToast();
			toast.ChangeDisplayedValues(message, sprite);
			toast.Show();
			ActiveToastsCount++;
		}

		private Toast getToast() {
			Toast result;
			if (!Pool.Any()) {
				result = Instantiate(toastPrefab, transform);
				result.SubscribeOnHidden(() => {
					ActiveToastsCount--;
					Pool.Enqueue(result);
				});
				return result;
			} else {
				result = Pool.Dequeue();
				result.transform.SetAsLastSibling();
			}

			return result;
		}
	}
}