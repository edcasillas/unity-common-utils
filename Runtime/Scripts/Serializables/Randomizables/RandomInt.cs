using System;

namespace CommonUtils.Serializables.Randomizables {
	[Serializable]
	public class RandomInt : AbstractRandomizable<int> {
		public override int Get() => UnityEngine.Random.Range(Min, Max + 1);
	}
}