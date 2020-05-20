using System.Collections.Generic;
using NUnit.Framework;

namespace CommonUtils.Tests.Editor {
	public class RandomListTests {
		[Test]
		public void RandomListGeneralTest() {
			var randomList = new RandomList<int>();
			var popped = new HashSet<int>();

			for (int i = 0; i < 10; i++) randomList.Push(i);
			Assert.AreEqual(10, randomList.Count);

			for (int i = 0; i < 10; i++) popped.Add(randomList.Pop());
			Assert.IsTrue(randomList.IsEmpty());

			for (int i = 0; i < 10; i++) {
				Assert.IsTrue(popped.Contains(i));
			}
		}
	}
}