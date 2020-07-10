using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace CommonUtils.Tests.Editor {
	public class IndexRandomizerTests : MonoBehaviour {
		[Test]
		public void CheckResults_NoAssert() {
			// Arrange
			var indexCount = 5;
			var passes = indexCount * 2;
			var toTake = 2;
			var indexRandomizer = new IndexRandomizer(indexCount);
			var counts = new Dictionary<int, int>();

			for (var i = 0; i < indexCount; i++) counts.Add(i, 0);

			// Act-Assert
			for (int i = 0; i < passes; i++) {
				var result = indexRandomizer.Take(toTake);
				var taken = new HashSet<int>();
				for (int j = 0; j < result.Length; j++) {
					counts[result[j]]++;

					// Check the indices are not repeated.
					Assert.IsFalse(taken.Contains(result[j]), $"Returned repeated indices: [{string.Join(", ", result)}]");
					taken.Add(result[j]);
				}
			}

			// Check (requires visual check)
			Debug.Log($"Executed {passes} times with an array of {indexCount} items taking {toTake}");
			Debug.Log(counts.AsJsonString());
			Assert.AreEqual(toTake * passes, counts.Sum(kvp => kvp.Value));
		}
	}
}