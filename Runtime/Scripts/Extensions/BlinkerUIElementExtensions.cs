using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UI.BlinkerUIElements;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace CommonUtils.Extensions {
	public static class BlinkerUIElementExtensions {
		private class blinkCoroutineInfo {
			public CoroutinerInstance Coroutiner;
			public bool ShouldStop;
		}

		private static readonly ConcurrentDictionary<IBlinkerUIElement, blinkCoroutineInfo> coroutines = new ConcurrentDictionary<IBlinkerUIElement, blinkCoroutineInfo>();

		public static IReadOnlyDictionary<Graphic, float> GetOriginalAlphaValues(this IBlinkerUIElement blinkerElement) => blinkerElement.BlinkerGraphics.ToDictionary(g => g, g => g.color.a);

		public static void StartBlinking(this IBlinkerUIElement blinkerElement) {
			if (!blinkerElement.IsValid()) {
				Debug.LogError($"{nameof(BlinkerUIElementExtensions)}.{nameof(StartBlinking)} was called with a null or invalid argument.");
				return;
			}
			//Debug.Log("Start");
			if (coroutines.ContainsKey(blinkerElement)) { // Already blinking
				//	Debug.Log("Already blinking");
				coroutines[blinkerElement].ShouldStop = false; // Make sure it's not flagged to stop.
				return;
			}

			coroutines.TryAdd(blinkerElement, new blinkCoroutineInfo());
			coroutines[blinkerElement].Coroutiner =
				Coroutiner.StartCoroutine(blink(blinkerElement), $"UIBlinkingCoroutiner - {blinkerElement.name}");
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
			foreach (var g in blinkerElement.BlinkerGraphics) {
				if (blinkerElement.OriginalAlphaValues == null) {
					Debug.Log(":D " + ((MonoBehaviour)blinkerElement).gameObject.name);
					continue;
				}
 				var c = g.color;
				g.color = new Color(c.r, c.g, c.b, restore ? blinkerElement.OriginalAlphaValues[g] : 0f);
			}
		}
	}
}