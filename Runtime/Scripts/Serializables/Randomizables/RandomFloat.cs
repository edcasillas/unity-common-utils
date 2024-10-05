using System;

namespace CommonUtils.Serializables.Randomizables {
	[Serializable]
	public class RandomFloat : AbstractRandomizable<float> {
		public RandomFloat() {}
		public RandomFloat(float defaultMin, float defaultMax) : base(defaultMin, defaultMax) { }

		public override float Get() => UnityEngine.Random.Range(Min, Max);
	}
}