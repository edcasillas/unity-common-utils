using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.Effects.ShakingTransform {
	[AddComponentMenu("Effects/Shaking Transform Controller")]
	public class ShakingTransformController : MonoBehaviour, IVerbosable { // Based on https://roystan.net/articles/camera-shake.html
		#region Inspector fields
#pragma warning disable 649
		[SerializeField] private ScriptableShakingTransformPreset[] presets;
		[SerializeField] private UnityEvent onShakeFinished;
		[SerializeField] private bool verbose;
#pragma warning restore 649
		#endregion

		#region Properties and backing fields
		public float Seed { get; private set; }

		/// <summary>
		/// Controls shake magnitude with a decreasing value from 1 to 0; when it reaches 0, the transform is fully recovered from shaking.
		/// </summary>
		public float Trauma { get; private set; } = 0;

		public bool IsShaking { get; private set; }

		public bool IsPaused => !IsShaking && Trauma > 0;

		private IShakingTransformPreset currentPreset;
		public IShakingTransformPreset CurrentPreset {
			get {
				if (currentPreset == null) {
					if (presets.IsNullOrEmpty()) {
						Debug.LogWarning($"No shaking transform presets have been defined for {name}. A default preset will be created and used. For better results, assign at least one preset.", this);
						currentPreset = ScriptableObject.CreateInstance<ScriptableShakingTransformPreset>();
						currentPreset.name = "Default preset";
					} else {
						currentPreset = presets[0];
					}
				}

				return currentPreset;
			}
			private set => currentPreset = value;
		}

		private Dictionary<string, ScriptableShakingTransformPreset> presetsByName;

		public IReadOnlyDictionary<string, ScriptableShakingTransformPreset> Presets {
			get {
				if (presetsByName == null) ReloadPresets();
				return presetsByName;
			}
		}

		public bool IsVerbose => verbose;
		#endregion

		private void Awake() => Seed = Random.value;

		public void InduceStress(IShakingTransformPreset preset, float stress = 1f) {
			CurrentPreset = preset;
			InduceStress(stress);
		}

		public void InduceStress(float stress = 1f, string presetName = null) {
			Trauma = Mathf.Clamp01(Trauma + stress);
			if (!string.IsNullOrEmpty(presetName)) {
				if(Presets.ContainsKey(presetName)) CurrentPreset = Presets[presetName];
				else Debug.LogError($"Preset {presetName} is not defined.", this);
			}
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

		public void ReloadPresets() => presetsByName = presets.ToDictionary(p => p.name, p => p);

		private IEnumerator shake() {
			this.DebugLog("Started shaking");
			IsShaking = true;

			do {
				var shakePower = Mathf.Pow(Trauma, CurrentPreset.Magnitude);

				transform.localPosition = new Vector3(
					CurrentPreset.Intensity.x * (Mathf.PerlinNoise(Seed, Time.time * CurrentPreset.Frequency) * 2 - 1),
					CurrentPreset.Intensity.y * (Mathf.PerlinNoise(Seed + 1, Time.time * CurrentPreset.Frequency) * 2 - 1),
					CurrentPreset.Intensity.z * (Mathf.PerlinNoise(Seed + 2, Time.time * CurrentPreset.Frequency) * 2 - 1)
				) * shakePower;

				transform.localRotation = Quaternion.Euler(new Vector3(
					CurrentPreset.AngularIntensity.x * (Mathf.PerlinNoise(Seed + 3, Time.time * CurrentPreset.Frequency) * 2 - 1),
					CurrentPreset.AngularIntensity.y * (Mathf.PerlinNoise(Seed + 4, Time.time * CurrentPreset.Frequency) * 2 - 1),
					CurrentPreset.AngularIntensity.z * (Mathf.PerlinNoise(Seed + 5, Time.time * CurrentPreset.Frequency) * 2 - 1)
				) * shakePower);

				Trauma = Mathf.Clamp01(Trauma - CurrentPreset.RecoverySpeed * Time.deltaTime);
				yield return null;
			} while (Trauma > 0 || (transform.localPosition != Vector3.zero || transform.localRotation != Quaternion.identity));

			IsShaking = false;
			this.DebugLog("Finished shaking");
			onShakeFinished.Invoke();
		}
	}
}