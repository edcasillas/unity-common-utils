﻿using System;
using System.Collections.Generic;
using System.IO;
using CommonUtils.Extensions;
using CommonUtils.Heaps;
using NUnit.Framework;
using UnityEngine;

namespace CommonUtils.Tests.Editor.Heaps {
	public class DynamicPriorityQueueTests {
		private class Queueable : IComparable<Queueable> {
			public int Id;
			public int Priority;

			public int CompareTo(Queueable other) {
				if (ReferenceEquals(this, other))
					return 0;
				if (ReferenceEquals(null, other))
					return 1;
				return Priority.CompareTo(other.Priority);
			}

			public override string ToString() => $"[{Id},{Priority}]";
		}

		[Test]
		public void EnqueueDequeueOne() {
			// Arrange
			var heap = new DynamicPriorityQueue<Queueable>();
			var queueable = new Queueable {Id = 0, Priority = 0};

			// Act-Assert
			heap.Enqueue(queueable);
			var result = heap.Dequeue();

			Assert.AreEqual(queueable, result);
			Assert.AreEqual(0, heap.Count);
		}

		[Test]
		public void RandomlyInsertedItemsWithPrioritiesChanged() {
			// Arrange
			var heap = new DynamicPriorityQueue<Queueable>();
			var queueables = new List<Queueable> {
				new Queueable{Id = 0, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 1, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 2, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 3, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 4, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 5, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 6, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 7, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 8, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 9, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
			};

			var expectedOutputIndices = new[] {9, 7, 5, 3, 1, 0, 2, 4, 6, 8}; // Priorities will be set into this order.

			// Act - Enqueue values
			foreach (var queueable in queueables) {
				heap.Enqueue(queueable);
			}

			Debug.Log($"Heap before change of priorities: {heap.DataToString()}");

			// Update priorities
			for (int i = 0; i < expectedOutputIndices.Length; i++) {
				queueables[expectedOutputIndices[i]].Priority = i + 1;
				heap.Enqueue(queueables[expectedOutputIndices[i]]); // Call enqueue again to inform it's priority changed.
			}

			Debug.Log($"Heap after change of priorities: {heap.DataToString()}");
			var orderOfDequeue = new List<int>();

			// Assert
			for (var i = 0; i < expectedOutputIndices.Length; i++) { // Check items come out in the selected priority.
				var dequeued = heap.Dequeue();
				Assert.AreEqual(queueables[expectedOutputIndices[i]], dequeued);
				orderOfDequeue.Add(dequeued.Id);
			}
			Assert.IsTrue(heap.IsEmpty); // In the end, the queue must be empty.
			Debug.Log($"Order of dequeued: {string.Join(", ", orderOfDequeue)}");
		}

		[Test]
		public void IndicesAreValidAfterDequeue() {
			// Arrange
			var heap = new DynamicPriorityQueue<Queueable>();
			var queueables = new List<Queueable> {
				new Queueable{Id = 0, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 1, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 2, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 3, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 4, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 5, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 6, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 7, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 8, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
				new Queueable{Id = 9, Priority = UnityEngine.Random.Range(int.MinValue, int.MaxValue)},
			};

			foreach (var queueable in queueables) {
				heap.Enqueue(queueable);
			}

			// Update priorities
			queueables[0].Priority = 6;
			heap.Enqueue(queueables[0]);

			queueables[1].Priority = 5;
			heap.Enqueue(queueables[1]);

			queueables[2].Priority = 7;
			heap.Enqueue(queueables[2]);

			queueables[3].Priority = 4;
			heap.Enqueue(queueables[3]);

			queueables[4].Priority = 8;
			heap.Enqueue(queueables[4]);

			queueables[5].Priority = 3;
			heap.Enqueue(queueables[5]);

			queueables[6].Priority = 9;
			heap.Enqueue(queueables[6]);

			queueables[7].Priority = 2;
			heap.Enqueue(queueables[7]);

			queueables[8].Priority = 10;
			heap.Enqueue(queueables[8]);

			queueables[9].Priority = 1;
			heap.Enqueue(queueables[9]);

			// Act
			heap.Dequeue();

			// Assert
			Assert.IsTrue(indicesAreValid(heap));
		}

		[Test]
		public void HeapValidity() {
			// Arrange
			var heap = new DynamicPriorityQueue<Queueable>();
			var queueables = new List<Queueable> {
				new Queueable{Id = 1, Priority = 134}, // 4th update
				new Queueable{Id = 2, Priority = 134}, // 1st update
				new Queueable{Id = 3, Priority = 134}, // 2nd update
				new Queueable{Id = 4, Priority = 134}, // 3rd update
				new Queueable{Id = 5, Priority = 134},
				new Queueable{Id = 6, Priority = 134},
				new Queueable{Id = 7, Priority = 134},
				new Queueable{Id = 8, Priority = 134},
				new Queueable{Id = 9, Priority = 134},
			};

			// Act
			foreach (var queueable in queueables) {
				heap.Enqueue(queueable);
			}

			// Update priorities
			queueables[1].Priority = 135;
			heap.Enqueue(queueables[1]);
			Assert.IsTrue(isValidHeap(heap.UnderlyingData), "Failed at 1st update");

			queueables[2].Priority = 135;
			heap.Enqueue(queueables[2]);
			Assert.IsTrue(isValidHeap(heap.UnderlyingData), "Failed at 2nd update");

			queueables[3].Priority = 135;
			heap.Enqueue(queueables[3]);
			Assert.IsTrue(isValidHeap(heap.UnderlyingData), "Failed at 3rd update");

			var tree = heap.StringifyTree();

			queueables[0].Priority = 135;
			heap.Enqueue(queueables[0]);

			tree = heap.StringifyTree();

			Assert.IsTrue(isValidHeap(heap.UnderlyingData), "Failed at 4th update");
		}

		[Test]
		public void TestCaseFromFile() {
			const string path =  "Packages/CommonUtils/Tests/Editor/Heaps/testcase.txt";

			if(!File.Exists(path)) Assert.Inconclusive("Couldn't find a file for the test case.");

			var heap = new DynamicPriorityQueue<Queueable>();
			var added = new Dictionary<int, Queueable>();

			var e = 0;
			var u = 0;
			var d = 0;

			//Read the text from directly from the test.txt file
			using (var reader = new StreamReader(path)) {
				while (!reader.EndOfStream) {
					var line = reader.ReadLine();
					var args = line.Split(' ');
					switch (args[0]) {
						case "E":
							var id = int.Parse(args[1]);
							var priority = int.Parse(args[2]);
							var prevPriority = -1;

							bool isUpdate = false;
							Queueable enqueueable;
							if (added.ContainsKey(id)) {
								enqueueable = added[id];
								prevPriority = enqueueable.Priority;
								enqueueable.Priority = priority;
								u++;
								isUpdate = true;
							} else {
								enqueueable = new Queueable {Id = id, Priority = priority};
								added.Add(id, enqueueable);
								e++;
							}

							try {
								if (isUpdate) {
									var tree = heap.StringifyTree();
								}
								heap.Enqueue(enqueueable);
								if (isUpdate) {
									var tree = heap.StringifyTree();
								}
							} catch {
								Debug.Log($"Error {(isUpdate ? "updating" : "adding")} {id}");
								Debug.Log($"{e}/{u}/{d}");
								Debug.Log($"Added: {added.Count}; Actual count: {heap.Count}");

								var s = heap.StringifyTree();
								throw;
							}
							break;
						case "D":
							d++;
							var dequeued = heap.Dequeue();
							if (!added.ContainsKey(dequeued.Id)) {
								Debug.Log($"{e}/{u}/{d}");
								Assert.Fail("The item dequeued was not in the dictionary.");
							}

							added.Remove(dequeued.Id);
							break;
						default:
							// Error
							break;

					}

					try {
						if (!isValidHeap(heap)) {
							Debug.Log($"{e}/{u}/{d}");
							Assert.Fail($"Heap became invalid after \"{line}\".");
						}
					} catch (Exception ex) {
						Debug.Log($"{e}/{u}/{d}");
						Assert.Fail($"Heap validation failed at line {line} with exception: {ex.Message}");
					}

					if (!indicesAreValid(heap)) {
						Debug.Log($"{e}/{u}/{d}");
						Assert.Fail($"Indices became invalid after \"{line}\".");
					}
				}

				reader.Close();
			}

			//Debug.Log($"{e}/{u}/{d}");
			Assert.AreEqual(added.Count, heap.Count);
		}

		private bool isValidHeap(PriorityQueue<Queueable> heap, int index = 0) {
			if (heap.IsEmpty || index > heap.Count) return true;
			var leftIndex = PriorityQueue<Queueable>.GetLeftChildIndex(index);
			var rightIndex = leftIndex + 1;

			if (leftIndex < heap.Count) {
				if (heap.UnderlyingData[leftIndex].Priority < heap.UnderlyingData[index].Priority || !isValidHeap(heap, leftIndex)) return false;
			}

			if (rightIndex < heap.Count) {
				if (heap.UnderlyingData[rightIndex].Priority < heap.UnderlyingData[index].Priority || !isValidHeap(heap, rightIndex)) return false;
			}

			return true;
		}

		private bool isValidHeap(IReadOnlyList<Queueable> arr, int index = 0) {
			if (arr.IsNullOrEmpty() || index > arr.Count) return true;
			var leftIndex = PriorityQueue<Queueable>.GetLeftChildIndex(index);
			var rightIndex = leftIndex + 1;

			if (leftIndex < arr.Count) {
				if (arr[leftIndex].Priority < arr[index].Priority || !isValidHeap(arr, leftIndex)) return false;
			}

			if (rightIndex < arr.Count) {
				if (arr[rightIndex].Priority < arr[index].Priority || !isValidHeap(arr, rightIndex)) return false;
			}

			return true;
		}

		private bool indicesAreValid(DynamicPriorityQueue<Queueable> queue) {
			if (queue.Count != queue.IndexOf.Count) return false;
			for (var i = 0; i < queue.Count; i++) {
				if (queue.IndexOf[queue.UnderlyingData[i]] != i) return false;
			}

			return true;
		}
	}
}