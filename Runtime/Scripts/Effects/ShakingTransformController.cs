using UnityEngine;

namespace CommonUtils.Effects {
	[AddComponentMenu("Effects/Shaking Transform Controller")]
	public class ShakingTransformController : MonoBehaviour { // Based on https://roystan.net/articles/camera-shake.html
		[Tooltip("Defines the maximum translation at each axis.")]
		[SerializeField] private Vector3 intensity = Vector3.one * 0.5f;

		[Tooltip("Defines the maximum rotation at each axis.")]
		[SerializeField] private Vector3 angularIntensity = Vector3.one * 0.5f;

		[Tooltip("How fast is the shake?")]
		[SerializeField] private float frequency = 25; // noise

		[SerializeField] private float recoverySpeed = 1.5f;

		[Tooltip("Multiplier to control the smooth falloff of the shake.")]
		[SerializeField] private float magnitude = 2f; // traumaExponent

		private float seed;

		// We set trauma to 1 to trigger an impact when the scene is run,
		// for debug purposes. This will later be changed to initialize trauma at 0.

		/// <summary>
		/// Controls shake magnitude with a decreasing value from 1 to 0; when it reaches 0, the transform is fully recovered from shaking.
		/// </summary>
		private float trauma = 0;

		private void Awake() => seed = Random.value;

		private void Update() { // TODO Improve to use a coroutine instead of the Update method so it doesn't execute when not shaking.
			//if(trauma <= 0) return; // Can't do this, or else the transform never returns to its original position and rotation.
			var shake = Mathf.Pow(trauma, magnitude);

			transform.localPosition = new Vector3(
				intensity.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
				intensity.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
				intensity.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
			) * shake;

			transform.localRotation = Quaternion.Euler(new Vector3(
				angularIntensity.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
				angularIntensity.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
				angularIntensity.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
			) * shake);

			trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
		}

		public void InduceStress(float stress) => trauma = Mathf.Clamp01(trauma + stress);

		public void InduceExplosionStress(Vector3 explosionPoint, float explosionRadius, float maximumStress = 0.6f) {
			// Example usage of InduceStress for an explosion.
			var distance = Vector3.Distance(explosionPoint, transform.position);
			var distance01 = Mathf.Clamp01(distance / explosionRadius);
			var stress = (1 - Mathf.Pow(distance01, 2)) * maximumStress;
			InduceStress(stress);
		}

		#if UNITY_EDITOR
		[ContextMenu("Shake it baby!")]
		private void shake() {
			InduceStress(1);
		}
		#endif
	}
}