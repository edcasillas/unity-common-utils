using System;

namespace CommonUtils.Serializables.Randomizables {
	[Serializable]
	public class RandomInt : AbstractRandomizable<int> {
		public RandomInt() {}
		public RandomInt(int defaultMin, int defaultMax) : base(defaultMin, defaultMax) { }

		public override int Get() => UnityEngine.Random.Range(Min, Max + 1);
	}
}