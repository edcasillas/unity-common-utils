using UnityEngine;

namespace CommonUtils.Effects.ShakingTransform {
	public class ShakingTransformPreset : IShakingTransformPreset {
		public Vector3 Intensity { get; set; }
		public Vector3 AngularIntensity { get; set; }
		public float Frequency { get; set; }
		public float RecoverySpeed { get; set; }
		public float Magnitude { get; set; }
		public string name { get; set; }
	}
}