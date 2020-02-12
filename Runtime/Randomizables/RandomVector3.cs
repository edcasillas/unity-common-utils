using System;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Randomizables {
	[Serializable]
	public class RandomVector3 : AbstractRandomizable<Vector3> {
		public override Vector3 Get() => Vector3Extensions.RandomRange(Min, Max);
	}
}