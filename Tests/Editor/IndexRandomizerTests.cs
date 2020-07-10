using System.Collections.Generic;
using CommonUtils.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace CommonUtils.Tests.Editor {
	public class IndexRandomizerTests : MonoBehaviour {
		[Test]
		public void CheckResults_NoAssert() {
			// Arrange
			var indexCount = 18;
			var passes = indexCount * 2;
			var indexRandomizer = new IndexRandomizer(indexCount);
			var counts = new Dictionary<int, int>();

			for (var i = 0; i < indexCount; i++) counts.Add(i, 0);

			// Act
			for (int i = 0; i < passes; i++) {
				var result = indexRandomizer.Take(3);
				for (int j = 0; j < result.Length; j++) {
					counts[result[j]]++;
				}
			}

			// Check (requires visual check)
			Debug.Log(counts.AsJsonString());
		}
	}
}