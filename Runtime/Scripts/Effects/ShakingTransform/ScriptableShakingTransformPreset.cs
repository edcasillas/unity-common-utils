using UnityEngine;

namespace CommonUtils.Effects.ShakingTransform {
	[CreateAssetMenu(menuName = "Common Utils/Shaking Transform Preset")]
	public class ScriptableShakingTransformPreset : ScriptableObject, IShakingTransformPreset {
		#region Inspector fields
		[Tooltip("Defines the maximum translation at each axis.")]
		[SerializeField] private Vector3 intensity = Vector3.one * 0.5f;

		[Tooltip("Defines the maximum rotation at each axis.")]
		[SerializeField] private Vector3 angularIntensity = Vector3.one * 0.5f;

		[Tooltip("How fast is the shake?")]
		[SerializeField] private float frequency = 25; // noise

		[Tooltip("How fast will the shake stop? The higher the value, the faster it will stop.")]
		[SerializeField] private float recoverySpeed = 1.5f;

		[Tooltip("Multiplier to control the smooth falloff of the shake.")]
		[SerializeField] private float magnitude = 2f; // traumaExponent
		#endregion

		public Vector3 Intensity => intensity;
		public Vector3 AngularIntensity => angularIntensity;
		public float Frequency => frequency;
		public float RecoverySpeed => recoverySpeed;
		public float Magnitude => magnitude;
	}
}
