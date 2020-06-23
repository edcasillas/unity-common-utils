using System.Collections;
using CommonUtils.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CommonUtils.Tests.PlayMode.Extensions {
	public class TransformExtensionsTests {
		[UnityTest]
		public IEnumerator GetChildren_Passes() {
			// Arrange
			var parent = GameObject.Instantiate(new GameObject());

			for (int i = 0; i < 3; i++) {
				GameObject.Instantiate(new GameObject(), parent.transform);
			}
			yield return null;

			// Act
			var children = parent.transform.GetChildren();

			// Assert
			Assert.AreEqual(parent.transform.childCount, children.Length);
			Assert.AreEqual(3, children.Length);

			// Tear down
			GameObject.Destroy(parent);
		}

		[UnityTest]
		public IEnumerator RemoveChildren_WithoutWhereCondition_Passes() {
			// Arrange
			var parent = GameObject.Instantiate(new GameObject());

			for (int i = 0; i < 3; i++) {
				var child = GameObject.Instantiate(new GameObject(), parent.transform);
			}
			yield return null;

			// Act
			var removeCount = parent.transform.RemoveChildren();

			yield return null; // Wait until the next frame so objects are actually destroyed.

			// Assert
			Assert.AreEqual(3, removeCount);
			Assert.AreEqual(0, parent.transform.childCount);

			// Tear down
			GameObject.Destroy(parent);
		}
	}
}