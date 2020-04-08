using UnityEngine;

namespace CommonUtils.Effects {
	public class ShakingTransformController : MonoBehaviour {
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
		private float trauma = 1;

		private void Awake() => seed = Random.value;

		private void Update() {
			float shake = Mathf.Pow(trauma, magnitude);

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

		/// <summary>
		///
		/// </summary>
		/// <param name="stress"></param>
		/// <example>
		/// [SerializeField] private float range = 45; // radius of explosion
		/// [SerializeField] private float maximumStress = 0.6f;
		///
		/// float distance = Vector3.Distance(transform.position, target.transform.position);
		/// float distance01 = Mathf.Clamp01(distance / range);
		/// float stress = (1 - Mathf.Pow(distance01, 2)) * maximumStress;
		/// target.InduceStress(stress); // target is a ShakingTransformController
		/// </example>
		public void InduceStress(float stress) => trauma = Mathf.Clamp01(trauma + stress);

		#if UNITY_EDITOR
		[ContextMenu("Shake it baby!")]
		private void shake() {
			InduceStress(1);
		}
		#endif
	}
}