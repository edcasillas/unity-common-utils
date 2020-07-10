using System;

namespace CommonUtils.Serializables.Randomizables {
	[Serializable]
	public class RandomFloat : AbstractRandomizable<float> {
		public override float Get() => UnityEngine.Random.Range(Min, Max);
	}
}