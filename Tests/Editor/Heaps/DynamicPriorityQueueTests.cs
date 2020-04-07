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
				new Queueable{Id = 0, Priority = 1},
				new Queueable{Id = 1, Priority = 1},
				new Queueable{Id = 2, Priority = 1},
				new Queueable{Id = 3, Priority = 2},
				new Queueable{Id = 4, Priority = 2},
				new Queueable{Id = 5, Priority = 2},
				new Queueable{Id = 6, Priority = 3},
				new Queueable{Id = 7, Priority = 3},
				new Queueable{Id = 8, Priority = 3},
				new Queueable{Id = 9, Priority = 4},
			};

			// Act
			foreach (var queueable in queueables) {
				heap.Enqueue(queueable);
			}

			heap.Dequeue(); // Dequeue 0

			// Update priority of 2
			queueables[2].Priority = 5;
			heap.Enqueue(queueables[2]);

			var stringified = heap.StringifyTree();

			// Assert
			Assert.AreEqual(queueables[0], heap.Peek());
			Assert.AreEqual(queueables[0], heap.Dequeue());
		}
	}
}