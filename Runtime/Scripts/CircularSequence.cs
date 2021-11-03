using System;

namespace ExaGames.Common {
	/// <summary>
	/// Handles a circular sequence, starting at zero and finishing at the specified maximum length.
	/// </summary>
	public class CircularSequence {
		private readonly int maxLength;
		private int currentIndex;

		/// <summary>
		/// Creates a new <see cref="CircularSequence"/> from zero to <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="maxLength">Max length of the sequence; when <see cref="Current"/> arrives to this number, it will be reseted to zero.</param>
		/// <param name="startIndex">Optional index to start the sequence. Default value is -1, so that <see cref="Next"/> retrieves zero (the first index in the sequence).</param>
		public CircularSequence(int maxLength, int startIndex = -1) {
			this.maxLength = maxLength;

			if (startIndex < -1) throw new ArgumentException("Can't start a circular sequence with an index less than -1", "startIndex");
			if(startIndex!=-1 && startIndex != maxLength) {
				startIndex = startIndex % maxLength;
			}
			
			currentIndex = startIndex;
		}

		/// <summary>
		/// Gets the next index in the sequence from <see cref="Current"/>, and updates <see cref="Current"/>.
		/// </summary>
		/// <returns>Next index in the sequence.</returns>
		public int Next() {
			currentIndex = Next(currentIndex);
			return currentIndex;
		}

		/// <summary>
		/// Gets the index next to <paramref name="index"/> without updating <see cref="Current"/>.
		/// </summary>
		/// <param name="index">Reference index to find its next.</param>
		/// <returns>Index next to <paramref name="index"/>.</returns>
		public int Next(int index) { return Next(index, maxLength); }

		/// <summary>
		/// Gets the previous index in the sequence from <see cref="Current"/>, and updates <see cref="Current"/>.
		/// </summary>
		/// <returns>Previous index in the sequence.</returns>
		public int Previous() {
			currentIndex = Previous(currentIndex);
			return currentIndex;
		}

		/// <summary>
		/// Gets the previous index to <paramref name="index"/> without updating <see cref="Current"/>.
		/// </summary>
		/// <param name="index">Reference index to find its previous.</param>
		/// <returns>Index previous to <paramref name="index"/>.</returns>
		public int Previous(int index) { return Previous(index, maxLength); }

		/// <summary>
		/// Gets the current index in this sequence.
		/// </summary>
		/// <returns>Current index in the sequence.</returns>
		public int Current() { return currentIndex; }

		/// <summary>
		/// Returns the next index in a circular sequence starting at <paramref name="min"/> and ending at <paramref name="max"/>.
		/// </summary>
		/// <param name="index">Current index for which to obtain its next.</param>
		/// <param name="max">Maximum number in the circular sequence.</param>
		/// <param name="min">Minimum number in the circular sequence.</param>
		/// <returns><paramref name="index"/> plus one, or <paramref name="min"/> if the <paramref name="index"/> reached <paramref name="max"/>.</returns>
		public static int Next(int index, int max, int min = 0) {
			index = index % max;
			return index + 1 == max ? 0 : index + 1;
		}
		public static int Previous(int index, int max, int min = 0) {
			index = index % max;
			return (index <= min ? max : index) - 1;
		}
	}
}