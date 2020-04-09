using UnityEngine;

namespace CommonUtils.Effects.ShakingTransform {
	[CreateAssetMenu(menuName = "Shaking Transform Preset")]
	public class ShakingTransformPreset : ScriptableObject {
		#region Inspector fields
		[Tooltip("Defines the maximum translation at each axis.")]
		[SerializeField] private Vector3 intensity = Vector3.one * 0.5f;

		[Tooltip("Defines the maximum rotation at each axis.")]
		[SerializeField] private Vector3 angularIntensity = Vector3.one * 0.5f;

		[Tooltip("How fast is the shake?")]
		[SerializeField] private float frequency = 25; // noise

		[SerializeField] private float recoverySpeed = 1.5f;

		[Tooltip("Multiplier to control the smooth falloff of the shake.")]
		[SerializeField] private float magnitude = 2f; // traumaExponent
		#endregion

		public Vector3 Intensity { get => intensity; set => intensity = value; }
		public Vector3 AngularIntensity { get => angularIntensity; set => angularIntensity = value; }
		public float Frequency { get => frequency; set => frequency = value; }
		public float RecoverySpeed { get => recoverySpeed; set => recoverySpeed = value; }
		public float Magnitude { get => magnitude; set => magnitude = value; }
	}
}
