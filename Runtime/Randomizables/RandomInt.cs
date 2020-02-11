using System;

namespace CommonUtils.Randomizables {
	[Serializable]
	public class RandomInt : AbstractRandomizable<int> {
		public override int Get() => UnityEngine.Random.Range(Min, Max + 1);
	}
}