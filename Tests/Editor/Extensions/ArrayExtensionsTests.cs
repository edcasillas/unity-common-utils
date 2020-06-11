using CommonUtils.Extensions;
using NUnit.Framework;

namespace CommonUtils.Tests.Editor.Extensions {
	public class ArrayExtensionsTests {
		[Test]
		public void BinarySearchClosestLowerIndex_TestCase0() {
			var array = new float[] {-4.1f, 0.1f, 2.0f, 8.0f, 10.5f, 12f};
			var targetValue = 0f;

			var result = array.BinarySearchClosestLowerIndex(targetValue);

			Assert.AreEqual(0, result);
		}

		[Test]
		public void BinarySearchClosestLowerIndex_TestCase1() {
			var array = new float[] {-4.1f, 0.1f, 2.0f, 8.0f, 10.5f, 12f};
			var targetValue = .5f;

			var result = array.BinarySearchClosestLowerIndex(targetValue);

			Assert.AreEqual(1, result);
		}

		[Test]
		public void BinarySearchClosestLowerIndex_TestCase2() {
			var array = new float[] {-4.1f, 0.1f, 2.0f, 8.0f, 10.5f, 12f};
			var targetValue = -10.5f;

			var result = array.BinarySearchClosestLowerIndex(targetValue);

			Assert.AreEqual(0, result);
		}

		[Test]
		public void BinarySearchClosestLowerIndex_TestCase3() {
			var array = new float[] {-4.1f, 0.1f, 2.0f, 8.0f, 10.5f, 12f};
			var targetValue = 11.5f;

			var result = array.BinarySearchClosestLowerIndex(targetValue);

			Assert.AreEqual(4, result);
		}

		[Test]
		public void BinarySearchClosestLowerIndex_TestCase4() {
			var array = new float[] {-4.1f, 0.1f, 2.0f, 8.0f, 10.5f, 12f};
			var targetValue = 15f;

			var result = array.BinarySearchClosestLowerIndex(targetValue);

			Assert.AreEqual(5, result);
		}
	}
}