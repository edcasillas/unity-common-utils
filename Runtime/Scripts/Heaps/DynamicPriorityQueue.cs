using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils.Heaps {
	/// <summary>
	/// Extends <see cref="PriorityQueue{T}"/> to enable changing the priority of
	/// its elements and constant-time access to them.
	/// </summary>
	/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
	public class DynamicPriorityQueue<T> : PriorityQueue<T> where T : IComparable<T> {
		private readonly Dictionary<T,int> indexOf;

		public IReadOnlyDictionary<T, int> IndexOf => indexOf;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPriorityQueue{T}"/> class. An optional collection can be specified
		/// as argument to copy its elements to the queue and ensure sufficient capacity to accommodate the number of
		/// elements copied, but will throw an exception if elements are repeated.
		/// </summary>
		/// <param name="source">The collection whose elements are copied to the new <see cref="DynamicPriorityQueue{T}"/>.</param>
		/// <exception cref="ArgumentException">An item in <paramref name="source"/> is repeated.</exception>
		public DynamicPriorityQueue(IEnumerable<T> source = null) : base(source) {
			indexOf = source.IsNullOrEmpty() ? new Dictionary<T, int>() : new Dictionary<T, int>(source.Count());
			for (var i = 0; i < Count; i++) {
				if (indexOf.ContainsKey(Data[i]))
					throw new ArgumentException($"Elements in a {nameof(DynamicPriorityQueue<T>)} cannot be repeated.");

				indexOf.Add(Data[i], i);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPriorityQueue{T}"/> class with the specified <paramref name="capacity"/>.
		/// </summary>
		/// <param name="capacity"></param>
		public DynamicPriorityQueue(int capacity) : base(capacity) => indexOf = new Dictionary<T, int>(capacity);

		/// <summary>
		/// Adds an object to the queue. If the object already exists, checks and fixes the priority of the queue.
		/// </summary>
		/// <param name="item">The object to add or update into the queue.</param>
		public override void Enqueue(T item) {
			if (!indexOf.ContainsKey(item)) {
				indexOf.Add(item, Count);
				base.Enqueue(item);
			}
			else {
				FixHeap(indexOf[item]);
			}
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the queue.</returns>
		public override T Dequeue() {
			var result = base.Dequeue();
			if (indexOf.ContainsKey(result)) {
				indexOf.Remove(result);
			}

			if (!IsEmpty) indexOf[Data[0]] = 0; // Ensure the stored index of the new top is up to date, because if swap was not called it might end up being the last one in the queue.
			return result;
		}

		/// <summary>
		/// Determines whether an element is in the queue.
		/// </summary>
		/// <param name="item">The object to locate in the queue.</param>
		/// <returns><c>true</c> if <paramref name="item"/> is found in the queue; otherwise, <c>false</c>.</returns>
		public bool Contains(T item) => indexOf.ContainsKey(item);

		/// <summary>
		/// Removes the specified <paramref name="item"/> from the queue.
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <exception cref="InvalidOperationException">The specified <paramref name="item"/> does not exists in the queue.</exception>
		public void Remove(T item) {
			if(!indexOf.ContainsKey(item)) throw new InvalidOperationException($"The specified value does not exist in this {nameof(DynamicPriorityQueue<T>)}");
			RemoveAt(indexOf[item]);
			indexOf.Remove(item);
		}

		protected override void Swap(int i, int j) {
			base.Swap(i, j);

			// After swapping, update the indices stored for each object.
			indexOf[Data[i]] = i;
			indexOf[Data[j]] = j;
		}
	}
}