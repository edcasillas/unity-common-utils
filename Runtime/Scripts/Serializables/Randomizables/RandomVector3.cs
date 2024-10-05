using System;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Serializables.Randomizables {
	[Serializable]
	public class RandomVector3 : AbstractRandomizable<Vector3> {
		public RandomVector3() {}
		public RandomVector3(Vector3 defaultMin, Vector3 defaultMax) : base(defaultMin, defaultMax) { }

		public override Vector3 Get() => Vector3Extensions.RandomRange(Min, Max);
	}
}