using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils {
	/// <summary>
	/// A generic queue that guarantees every item in it is unique; that is, the same item cannot be enqueued twice.
	/// It also provides constant access to the <see cref="Contains"/> method.
	///
	/// Enqueue can be O(n)
	/// Dequeue is O(1)
	/// Contains is O(1)
	/// </summary>
	/// <typeparam name="T">Type of the items in the queue.</typeparam>
    public class UniqueItemsQueue<T> : IReadOnlyCollection<T>, ICollection where T : class {
		private readonly Queue<T> queue;
		private readonly HashSet<T> hashSet;

		/// <summary>
		/// Creates a queue with room for capacity objects. The default initial
		/// capacity and grow factor are used.
		/// </summary>
		public UniqueItemsQueue() {
			queue = new Queue<T>();
			hashSet = new HashSet<T>();
		}

		/// <summary>
		/// Creates a queue with room for capacity objects. The default grow factor is used.
		/// </summary>
		public UniqueItemsQueue(IEnumerable<T> collection) {
			queue = new Queue<T>(collection.ToList());
			hashSet = new HashSet<T>(queue);
		}

		/// <summary>
		/// Fills a Queue with the elements of an ICollection.  Uses the enumerator to get each of the elements.
		/// </summary>
		/// <param name="capacity"></param>
		public UniqueItemsQueue(int capacity) {
			queue = new Queue<T>(capacity);
			hashSet = new HashSet<T>();
		}

		/// <summary>
		/// Checks if the queue contains the item.
		/// </summary>
		public bool Contains(T item) => hashSet.Contains(item);

		/// <summary>
		/// Removes the object at the head of the queue and returns it. If the queue is empty, this method simply returns null.
		/// </summary>
		public T Dequeue() {
			var result = queue.Dequeue();
			if (result == null) return null;
			hashSet.Remove(result);
			return result;
		}

		/// <summary>
		/// Adds item to the tail of the queue, as long as the item is not already contained in the queue.
		/// </summary>
		/// <returns><c>true</c> when the item is not previously present in the queue and enqueueing was successful, otherwise <c>false</c>.</returns>
		public bool Enqueue(T item) {
			if(hashSet.Contains(item)) return false;
			queue.Enqueue(item);
			hashSet.Add(item);
			return true;
		}

		/// <summary>
		/// Returns the object at the head of the queue. The object remains in the queue. If the queue is empty, this method throws an
		/// InvalidOperationException.
		/// </summary>
		public T Peek() => queue.Peek();

		/// <summary>
		/// Iterates over the objects in the queue, returning an array of the
		/// objects in the Queue, or an empty array if the queue is empty.
		/// The order of elements in the array is first in to last in, the same
		/// order produced by successive calls to Dequeue.
		/// </summary>
		public T[] ToArray => queue.ToArray();

		public void TrimExcess() {
			queue.TrimExcess();
			hashSet.TrimExcess();
		}

#if UNITY_2022_1_OR_NEWER
		public bool TryDequeue(out T result) {
			if (!queue.TryDequeue(out result)) return false;
			hashSet.Remove(result);
			return true;
		}

		public bool TryPeek(out T result) => queue.TryPeek(out result);
#endif

		/// <summary>
		/// Removes all Objects from the queue.
		/// </summary>
		public void Clear() {
			queue.Clear();
			hashSet.Clear();
		}

		/// <summary>
		/// CopyTo copies a collection into an Array, starting at a particular index into the array.
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex) => queue.CopyTo(array, arrayIndex);

		#region Generic collection interfaces implementation
		public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		void ICollection.CopyTo(Array array, int index) => ((ICollection)queue).CopyTo(array, index);
		public int Count => queue.Count;
		public bool IsSynchronized => ((ICollection)queue).IsSynchronized;
		public object SyncRoot => ((ICollection)queue).SyncRoot;
		#endregion

	}
}
