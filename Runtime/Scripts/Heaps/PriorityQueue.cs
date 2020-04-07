using System;
using System.Collections.Generic;

namespace CommonUtils.Heaps {
	/// <summary>
	/// Implements a generic min-heap where the priority of its elements is given
	/// by their implementation of the <see cref="IComparable{T}"/> interface.
	/// </summary>
	/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
	public class PriorityQueue<T> where T : IComparable<T> {
		/// <summary>
		/// Underlying structure holding the items of the heap.
		/// </summary>
		protected readonly List<T> Data;

		/// <summary>
		/// Gets the number of elements contained in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		public int Count => Data.Count;

		/// <summary>
		/// Gets a value indicating whether the <see cref="PriorityQueue{T}"/> is empty.
		/// </summary>
		public bool IsEmpty => Count == 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class. An optional collection can be specified
		/// as argument to copy its elements to the queue and ensure sufficient capacity to accommodate the number of
		/// elements copied.
		/// </summary>
		/// <param name="source">The collection whose elements are copied to the new <see cref="PriorityQueue{T}"/>.</param>
		public PriorityQueue(IEnumerable<T> source = null) {
			Data = source == null ? new List<T>() : new List<T>(source);
			for (var i = Data.Count / 2 - 1; i >= 0; i--) heapifyDown(i);
		}

		/// <summary>
		/// Adds an object to the queue.
		/// </summary>
		/// <param name="item">The object to add to the queue.</param>
		public virtual void Enqueue(T item) {
			Data.Add(item);
			heapifyUp(getParentIndex(Data.Count - 1));
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the queue.</returns>
		public virtual T Dequeue() {
			if (Data.Count == 0) return default;
			var result = Data[0];

			if (Data.Count == 1) {
				Data.RemoveAt(0);
			} else {
				Data[0] = Data[Data.Count -1];
				Data.RemoveAt(Data.Count - 1);
				heapifyDown();
			}

			return result;
		}

		/// <summary>
		/// Returns the object at the beginning of the queue without removing it.
		/// </summary>
		/// <returns></returns>
		public T Peek() => Data.Count > 0 ? Data[0] : default;

		/// <summary>
		/// Removes an arbitrary item from the queue given its <paramref name="index"/> in the heap.
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		/// <exception cref="IndexOutOfRangeException">The specified <paramref name="index"/> is not valid.</exception>
		public void RemoveAt(int index) {
			if (Data.Count == 0) return;

			if (index < 0 || index >= Data.Count) throw new IndexOutOfRangeException();

			if (index == 0) {
				Dequeue();
				return;
			}

			Data[index] = Data[Data.Count -1];
			Data.RemoveAt(Data.Count - 1);

			FixHeap(index);
		}

		/// <summary>
		/// Fixes the heap assuming a conflict at the specified <paramref name="index"/> of the heap.
		/// If the priority of the item at <paramref name="index"/> is higher than its parent, bubbles up the item at <paramref name="index"/>,
		/// otherwise bubbles it down.
		/// </summary>
		/// <param name="index">Index of the heap that must be fixed.</param>
		protected void FixHeap(int index) {
			if (index > 0 && Data[getParentIndex(index)].CompareTo(Data[index]) < 0) {
				heapifyUp(getParentIndex(index));
			} else {
				heapifyDown(index);
			}
		}

		/// <summary>
		/// Swaps the contents of the two specified indices in the heap.
		/// </summary>
		protected virtual void Swap(int i, int j) {
			var tmp = Data[i];
			Data[i] = Data[j];
			Data[j] = tmp;
		}

		private void heapifyUp(int i) {
			if (i < 0 || i > Data.Count) return;
			var left = getLeftChildIndex(i);
			var right = getRightChildIndex(i);
			var min = i;
			if (left < Data.Count && Data[left].CompareTo(Data[min]) < 0) min = left;
			if (right < Data.Count && Data[right].CompareTo(Data[min]) < 0) min = right;
			if (min != i) {
				Swap(min, i);
				heapifyUp(getParentIndex(i));
			}
		}

		private void heapifyDown(int i = 0) {
			if (i < 0 || i > Data.Count) return;
			var left = getLeftChildIndex(i);
			var right = getRightChildIndex(i);
			var min = i;
			if (left < Data.Count && Data[left].CompareTo(Data[min]) < 0) min = left;
			if (right < Data.Count && Data[right].CompareTo(Data[min]) < 0) min = right;
			if (min != i) {
				Swap(min, i);
				heapifyDown(min);
			}
		}

		private static int getLeftChildIndex(int i) => (2 * i) + 1;
		private static int getRightChildIndex(int i) => (2 * i) + 2;
		private static int getParentIndex(int i) => (i - 1) / 2;
	}
}