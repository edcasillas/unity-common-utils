using CommonUtils.Coroutines;
using System.Collections;
using System.Collections.Concurrent;
using CommonUtils.UI.BlinkerUIElements;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class BlinkerUIElementExtensions {
		private class blinkCoroutineInfo {
			public CoroutinerInstance Coroutiner;
			public bool ShouldStop;
		}

		private static readonly ConcurrentDictionary<IBlinkerUIElement, blinkCoroutineInfo> coroutines = new ConcurrentDictionary<IBlinkerUIElement, blinkCoroutineInfo>();

		public static void StartBlinking(this IBlinkerUIElement blinkerElement) {
			if (!blinkerElement.IsValid()) {
				Debug.LogError($"{nameof(BlinkerUIElementExtensions)}.{nameof(StartBlinking)} was called with a null or invalid argument.");
				return;
			}

			if (!blinkerElement.CanvasGroup) {
				Debug.LogError($"Cannot start blinking on {blinkerElement.name} because it doesn't have a required reference to a {nameof(CanvasGroup)}");
				return;
			}

			//Debug.Log("Start");
			if (coroutines.ContainsKey(blinkerElement)) { // Already blinking
				//	Debug.Log("Already blinking");
				coroutines[blinkerElement].ShouldStop = false; // Make sure it's not flagged to stop.
				return;
			}

			coroutines.TryAdd(blinkerElement, new blinkCoroutineInfo());
			coroutines[blinkerElement].Coroutiner = Coroutiner.StartCoroutine(blink(blinkerElement), $"UIBlinkingCoroutiner - {blinkerElement.name}");
		}

		public static void StopBlinking(this IBlinkerUIElement blinkerElement) {
			if (!blinkerElement.IsValid()) {
				Debug.LogError($"{nameof(BlinkerUIElementExtensions)}.{nameof(StopBlinking)} was called with a null or invalid argument.");
				return;
			}

			//Debug.Log("Stop");
			if(!coroutines.ContainsKey(blinkerElement) ||coroutines[blinkerElement].ShouldStop) // Not blinking
				return;

			coroutines[blinkerElement].ShouldStop = true;
		}

		private static IEnumerator blink(IBlinkerUIElement blinkerElement) {
			//Debug.Log($"Blink starting");
			while (coroutines.ContainsKey(blinkerElement) && !coroutines[blinkerElement].ShouldStop) {
				yield return new WaitForSeconds(blinkerElement.BlinkingRate);
				blinkerElement.setAlpha(false);

				yield return new WaitForSeconds(blinkerElement.BlinkingRate);
				blinkerElement.setAlpha(true);
			}
			blinkerElement.setAlpha(true);

			blinkCoroutineInfo _;
			if (!coroutines.TryRemove(blinkerElement, out _)) {
				//Debug.Log($"Coroutine was not removed from dictionary");
			}
			//Debug.Log($"Blinking finished");
		}

		private static void setAlpha(this IBlinkerUIElement blinkerElement, bool restore) {
			//Debug.Log($"Setting alpha to {restore}");
			blinkerElement.CanvasGroup.alpha = restore ? 1f : 0f;
		}
	}
}