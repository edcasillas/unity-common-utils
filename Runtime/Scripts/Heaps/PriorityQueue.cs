﻿using System;
using System.Collections.Generic;
using System.Text;
using CommonUtils.Extensions;
using UnityEngine;

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

		internal IReadOnlyList<T> UnderlyingData => Data;

		/// <summary>
		/// Gets the number of elements contained in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		public int Count { get; private set; }

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
			Data = source.IsNullOrEmpty() ? new List<T>() : new List<T>(source);
			Count = Data.Count;
			for (var i = Count / 2 - 1; i >= 0; i--) heapifyDown(i);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class with the specified <paramref name="capacity"/>.
		/// </summary>
		/// <param name="capacity">Capacity of the queue.</param>
		/// <exception cref="ArgumentOutOfRangeException">Capacity is less than zero.</exception>
		public PriorityQueue(int capacity) {
			if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
			Data = new List<T>(capacity);
			Count = Data.Count;
		}

		/// <summary>
		/// Adds an object to the queue.
		/// </summary>
		/// <param name="item">The object to add to the queue.</param>
		public virtual void Enqueue(T item) {
			if (Count == Data.Count) {
				Data.Add(item);
				Count = Data.Count;
			} else {
				Data[Count] = item;
				Count++;
			}
			heapifyUp(GetParentIndex(Count - 1));
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the queue.</returns>
		public virtual T Dequeue() {
			if (Count == 0) return default;
			var result = Data[0];

			if (Count == 1) {
				Data[0] = default;
				Count--;
			} else {
				Data[0] = Data[Count -1];
				Data[Count - 1] = default;
				Count--;
				heapifyDown();
			}

			return result;
		}

		/// <summary>
		/// Returns the object at the beginning of the queue without removing it.
		/// </summary>
		/// <returns></returns>
		public T Peek() => Count > 0 ? Data[0] : default;

		/// <summary>
		/// Removes an arbitrary item from the queue given its <paramref name="index"/> in the heap.
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		/// <exception cref="IndexOutOfRangeException">The specified <paramref name="index"/> is not valid.</exception>
		public void RemoveAt(int index) {
			if (Count == 0) return;

			if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

			if (index == 0) {
				Dequeue();
				return;
			}

			Data[index] = Data[Count -1];
			Data[Count - 1] = default;
			Count--;

			FixHeap(index);
		}

		public string StringifyTree(Func<T, string> printItemDelegate = null, int printedItemLength = 0) {
			if (IsEmpty) return string.Empty;
			if (printItemDelegate == null) printItemDelegate = item => item.ToString();
			if (printedItemLength <= 0) printedItemLength = printItemDelegate(Data[Count - 1]).Length;

			var depth = getDepth();
			var strLevels = new string[depth + 1];
			var lastNodeInLevel = Count; // exclusive
			var spacesIncrement = printedItemLength.GetWhiteSpaces();
			var spaces = string.Empty;

			while(depth >= 0) {
				var initialNode = Mathf.RoundToInt(Mathf.Pow(2, depth) -1); // inclusive
				var levelStrBuilder = new StringBuilder();

				levelStrBuilder.Append(spaces);
				spaces = spaces + spaces + spacesIncrement;

				for(var i = initialNode; i < lastNodeInLevel; i++) {
					levelStrBuilder.Append($"{printItemDelegate(Data[i])}{spaces}");
				}
				strLevels[depth] = levelStrBuilder.ToString().TrimEnd();
				depth--;
				lastNodeInLevel = initialNode;
			}

			return string.Join("\n", strLevels);
		}

		public string DataToString() => string.Join(", ", Data);

		/// <summary>
		/// Fixes the heap assuming a conflict at the specified <paramref name="index"/> of the heap.
		/// If the priority of the item at <paramref name="index"/> is higher than its parent, bubbles up the item at <paramref name="index"/>,
		/// otherwise bubbles it down.
		/// </summary>
		/// <param name="index">Index of the heap that must be fixed.</param>
		protected void FixHeap(int index) {
			var parentIndex = GetParentIndex(index);
			if (parentIndex >= 0 && Data[parentIndex].CompareTo(Data[index]) < 0) { // The parent of the item at index has a higher priority than the item itself, so they're in the correct places and we need to heapify down.
				heapifyDown(index);
			} else {
				if(parentIndex >= 0) heapifyUp(GetParentIndex(index));
				else heapifyDown();
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
			if (i < 0 || i > Count) return;
			var left = GetLeftChildIndex(i);
			var right = GetRightChildIndex(i);
			var min = i;
			if (left < Count && Data[left].CompareTo(Data[min]) < 0) min = left;
			if (right < Count && Data[right].CompareTo(Data[min]) < 0) min = right;
			if (min != i) {
				Swap(min, i);
				heapifyUp(GetParentIndex(i));
			}
		}

		private void heapifyDown(int i = 0) {
			if (i < 0 || i > Count) return;
			var left = GetLeftChildIndex(i);
			var right = GetRightChildIndex(i);
			var min = i;
			if (left < Count && Data[left].CompareTo(Data[min]) < 0) min = left;
			if (right < Count && Data[right].CompareTo(Data[min]) < 0) min = right;
			if (min != i) {
				Swap(min, i);
				heapifyDown(min);
			}
		}

		// TODO Should these methods be internal instead of public?
		public static int GetLeftChildIndex(int i) => (2 * i) + 1;
		public static int GetRightChildIndex(int i) => (2 * i) + 2;
		public static int GetParentIndex(int i) => i > 0 ? (i - 1) / 2 : -1;

		private static int getLevel(int i) => Mathf.FloorToInt(Mathf.Log(i + 1, 2));
		private int getDepth() => getLevel(Count - 1);
	}
}