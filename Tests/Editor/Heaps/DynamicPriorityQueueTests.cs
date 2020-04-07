using System;
using System.Collections.Generic;
using CommonUtils.Heaps;
using NUnit.Framework;

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

		// A Test behaves as an ordinary method
		[Test]
		public void DynamicPriorityQueueTestsSimplePasses() {
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

				/*new Queueable{Id = 0, Priority = 9},
				new Queueable{Id = 1, Priority = 8},
				new Queueable{Id = 2, Priority = 7},
				new Queueable{Id = 3, Priority = 6},
				new Queueable{Id = 4, Priority = 5},
				new Queueable{Id = 5, Priority = 4},
				new Queueable{Id = 6, Priority = 3},
				new Queueable{Id = 7, Priority = 2},
				new Queueable{Id = 8, Priority = 1},
				new Queueable{Id = 9, Priority = 0},*/
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
	}
}