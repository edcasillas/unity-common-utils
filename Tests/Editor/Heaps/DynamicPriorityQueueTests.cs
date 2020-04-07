using System;
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

			// Act
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

			// Assert
			Assert.AreEqual(queueables[9], heap.Dequeue());
			Assert.AreEqual(queueables[7], heap.Dequeue());
			Assert.AreEqual(queueables[5], heap.Dequeue());
			Assert.AreEqual(queueables[3], heap.Dequeue());
			Assert.AreEqual(queueables[1], heap.Dequeue());
			Assert.AreEqual(queueables[0], heap.Dequeue());
			Assert.AreEqual(queueables[2], heap.Dequeue());
			Assert.AreEqual(queueables[4], heap.Dequeue());
			Assert.AreEqual(queueables[6], heap.Dequeue());
			Assert.AreEqual(queueables[8], heap.Dequeue());
			Assert.IsTrue(heap.IsEmpty);
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

					if (!isValidHeap(heap.UnderlyingData)) {
						Debug.Log($"{e}/{u}/{d}");
						Assert.Fail($"Heap became invalid after \"{line}\".");
					}

					if (!indicesAreValid(heap)) {
						Debug.Log($"{e}/{u}/{d}");
						Assert.Fail($"Indices became invalid after \"{line}\".");
					}
				}

				reader.Close();
			}

			Debug.Log($"{e}/{u}/{d}");
			Assert.AreEqual(added.Count, heap.Count);
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
			if (queue.UnderlyingData.Count != queue.IndexOf.Count) return false;
			for (var i = 0; i < queue.UnderlyingData.Count; i++) {
				if (queue.IndexOf[queue.UnderlyingData[i]] != i) return false;
			}

			return true;
		}
	}
}