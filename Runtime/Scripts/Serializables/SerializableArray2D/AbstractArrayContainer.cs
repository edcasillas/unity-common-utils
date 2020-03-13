using System;
using UnityEngine;

namespace CommonUtils.Serializables.SerializableArray2D {
	[Serializable]
	public abstract class AbstractArrayContainer<T> {
		[SerializeField] private T[] columns;

		public T this[int index] {
			get => columns[index];
			set => columns[index] = value;
		}

		public int Length => columns.Length;

		internal void SetSize(int size) => columns = new T[size];
	}
}