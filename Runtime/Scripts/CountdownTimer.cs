using System;

namespace CommonUtils {
	public class CountdownTimer {
		private readonly double totalTime;

		private int prevSeconds = -1;
		private int prevMinutes = -1;

		public CountdownTimer(double totalTime) => this.totalTime = totalTime;

		public double Update(double elapsed, Action<int, int> onTimerChanged) {
			var timeLeft = (float)(totalTime - elapsed);
			if (timeLeft < 0) return 0;

			var minutes = (int)(timeLeft / 60);
			var seconds = (int)(timeLeft % 60);

			// Only send the result to the callback when we know it's gonna change, not every frame.
			// This is to avoid unnecessary memory allocation which results in more expensive GC collections.
			if (prevSeconds != seconds || prevMinutes != minutes) {
				prevSeconds = seconds;
				prevMinutes = minutes;
				onTimerChanged.Invoke(minutes, seconds);
			}

			return timeLeft;
		}

		public void Reset() {
			prevSeconds = -1;
			prevMinutes = -1;
		}
	}
}