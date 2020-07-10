using System;

namespace CommonUtils {
	public class IndexRandomizer {
		private readonly RandomList<int> randomList;

		public IndexRandomizer(int count) {
			randomList = new RandomList<int>();
			for (var i = 0; i < count; i++) {
				randomList.Push(i);
			}
		}

		public int[] Take(int n) {
			var result = new int[n];
			Take(n, result);
			return result;
		}

		public void Take(int n, int[] output) {
			if (n < 0) throw new ArgumentException($"The amount of indices to take ({nameof(n)}) cannot be less than zero.");
			if(output == null) throw new ArgumentNullException(nameof(output));
			if (n > output.Length) throw new ArgumentException($"The amount of indices to take ({nameof(n)}) cannot be greater than or equal to the size of the output array.");

			for (var i = 0; i < n; i++) {
				var randomIndex = randomList.Pop((uint)i); // Pop a random index from the random list, excluding those that are already selected.
				randomList.Push(i); // Push it again so the index is available the next round.
				output[i] = randomIndex;
			}
		}
	}
}