using System;
using UnityEngine;

namespace CommonUtils.Randomizables {
	[Serializable]
	public abstract class AbstractRandomizable<T> {
#pragma warning disable 649
		[SerializeField] [Tooltip("Inclusive lower limit for this random item.")]
		private T min;

		[SerializeField] [Tooltip("Inclusive upper limit for this random item.")]
		private T max;
#pragma warning restore 649

		public T Min => min;
		public T Max => max;

		/// <summary>
		/// When overriden by a derived class, returns a random <typeparamref name="T"/> between <see cref="AbstractRandomizable{T}.Min"/> and <see cref="AbstractRandomizable{T}.Max"/>.
		/// </summary>
		public abstract T Get();
	}
}
//#pragma warning restore IDE0044