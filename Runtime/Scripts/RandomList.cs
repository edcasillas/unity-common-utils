using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonUtils.Extensions;

namespace CommonUtils {
	/// <summary>
	/// Allows to randomly select an item from a list, discard that item, and repeat with the remaining items.
	/// </summary>
	[DebuggerDisplay("{" + nameof(Count) + "}")]
	public class RandomList<T> {
		/// <summary>
		/// Doubly Linked List Node.
		/// </summary>
		[DebuggerDisplay("{" + nameof(Data) + "}")]
		private class Node {
			public T Data;
			public Node Previous;
			public Node Next;

			public Node(T data) {
				Data = data;
				Previous = null;
				Next = null;
			}
		}

		/// <summary>
		/// Start of the list.
		/// </summary>
		private Node start;
		/// <summary>
		/// End of the list.
		/// </summary>
		private Node end;

		/// <summary>
		/// Item count in this <see cref="RandomList{T}"/>.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Creates an empty <see cref="RandomList{T}"/>.
		/// </summary>
		public RandomList() { }

		/// <summary>
		/// Creates a <see cref="RandomList{T}"/> with the specified <paramref name="items"/>.
		/// </summary>
		/// <param name="items"></param>
		public RandomList(IEnumerable<T> items) {
			if(items.IsNullOrEmpty()) return;
			foreach (var item in items) Push(item);
		}

		/// <summary>
		/// Adds a new item to the <see cref="RandomList{T}"/>.
		/// </summary>
		/// <param name="item">Item to be added.</param>
		public void Push(T item) {
			var newNode = new Node(item);
			if(Count == 0) {
				start = end = newNode;
			} else {
				newNode.Previous = end;
				end.Next = newNode;
				end = newNode;
			}
			Count++;
		}

		/// <summary>
		/// Retrieves a random item from the list and removes it from the list.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		/// <param name="exclude">Items to exclude at the end of the list.</param>
		/// <param name="throwErrorOnEmpty">If set to <c>true</c> throw error when list is empty, otherwise returns the data default value.</param>
		public T Pop(uint exclude = 0, bool throwErrorOnEmpty = false) {
			if(Count > 0 && Count > exclude) {
				Node p = start;
				int index = UnityEngine.Random.Range(0, Count - (int)exclude);
				for(var i = 1; i <= index; i++) {
					p = p?.Next ?? throw new Exception("RandomList is corrupted.");
				}
				remove(p);
				return p.Data;
			}

			if(throwErrorOnEmpty) throw new Exception("No values available");
			return default;
		}

		/// <summary>
		/// Determines whether this random list, excluding the specified number of items, is empty.
		/// </summary>
		/// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
		/// <param name="exclude">Number of items to exclude.</param>
		public bool IsEmpty(uint exclude = 0) => Count - exclude <= 0;

		private void remove(Node node) {
			if(node == start) {
				start = start.Next;
				if(start != null) {
					start.Previous = null;
				} else {
					end = null;
				}
			} else if(node == end) {
				end = end.Previous;
				end.Next = null;
			} else {
				node.Next.Previous = node.Previous;
				node.Previous.Next = node.Next;
			}
			node.Next = node.Previous = null;
			Count--;
		}
	}
}