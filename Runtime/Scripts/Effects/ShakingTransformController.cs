using System.Collections;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Effects {
	[AddComponentMenu("Effects/Shaking Transform Controller")]
	public class ShakingTransformController : MonoBehaviour, IVerbosable { // Based on https://roystan.net/articles/camera-shake.html
		#region Inspector fields
#pragma warning disable 649
		[Tooltip("Defines the maximum translation at each axis.")]
		[SerializeField] private Vector3 intensity = Vector3.one * 0.5f;

		[Tooltip("Defines the maximum rotation at each axis.")]
		[SerializeField] private Vector3 angularIntensity = Vector3.one * 0.5f;

		[Tooltip("How fast is the shake?")]
		[SerializeField] private float frequency = 25; // noise

		[SerializeField] private float recoverySpeed = 1.5f;

		[Tooltip("Multiplier to control the smooth falloff of the shake.")]
		[SerializeField] private float magnitude = 2f; // traumaExponent

		[SerializeField] private bool verbose;
#pragma warning restore 649
		#endregion

		public float Seed { get; private set; }

		/// <summary>
		/// Controls shake magnitude with a decreasing value from 1 to 0; when it reaches 0, the transform is fully recovered from shaking.
		/// </summary>
		public float Trauma { get; private set; } = 0;

		public bool IsShaking { get; private set; }

		public bool IsPaused => !IsShaking && Trauma > 0;

		public bool IsVerbose => verbose;

		private void Awake() => Seed = Random.value;

		public void InduceStress(float stress) {
			Trauma = Mathf.Clamp01(Trauma + stress);
			if (!IsShaking) StartCoroutine(shake());
		}

		public void InduceExplosionStress(Vector3 explosionPoint, float explosionRadius, float maximumStress = 0.6f) {
			// Example usage of InduceStress for an explosion.
			var distance = Vector3.Distance(explosionPoint, transform.position);
			var distance01 = Mathf.Clamp01(distance / explosionRadius);
			var stress = (1 - Mathf.Pow(distance01, 2)) * maximumStress;
			InduceStress(stress);
		}

		public void Pause(bool restoreOrigin = false) {
			StopAllCoroutines();
			IsShaking = false;
			if (restoreOrigin) {
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		}

		public void Resume() => StartCoroutine(shake());

		public void Stop() {
			StopAllCoroutines();
			Trauma = 0;
			IsShaking = false;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		private IEnumerator shake() {
			this.DebugLog("Started shaking");
			IsShaking = true;

			do {
				var shakePower = Mathf.Pow(Trauma, magnitude);

				transform.localPosition = new Vector3(
					intensity.x * (Mathf.PerlinNoise(Seed, Time.time * frequency) * 2 - 1),
					intensity.y * (Mathf.PerlinNoise(Seed + 1, Time.time * frequency) * 2 - 1),
					intensity.z * (Mathf.PerlinNoise(Seed + 2, Time.time * frequency) * 2 - 1)
				) * shakePower;

				transform.localRotation = Quaternion.Euler(new Vector3(
					angularIntensity.x * (Mathf.PerlinNoise(Seed + 3, Time.time * frequency) * 2 - 1),
					angularIntensity.y * (Mathf.PerlinNoise(Seed + 4, Time.time * frequency) * 2 - 1),
					angularIntensity.z * (Mathf.PerlinNoise(Seed + 5, Time.time * frequency) * 2 - 1)
				) * shakePower);

				Trauma = Mathf.Clamp01(Trauma - recoverySpeed * Time.deltaTime);
				yield return null;
			} while (Trauma >= 0 && (transform.localPosition != Vector3.zero ||
									 transform.localRotation != Quaternion.identity));

			IsShaking = false;
			this.DebugLog("Finished shaking");
		}
	}
}