using UnityEngine;

namespace CommonUtils.Effects.ShakingTransform {
	public interface IShakingTransformPreset {
		Vector3 Intensity { get; }
		Vector3 AngularIntensity { get; }
		float Frequency { get; }
		float RecoverySpeed { get; }
		float Magnitude { get; }
		string name { get; set; }
	}
}