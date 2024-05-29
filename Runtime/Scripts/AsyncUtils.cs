using CommonUtils.Coroutines;
using System;
using System.Threading.Tasks;

namespace CommonUtils {
    public static class AsyncUtils {
		public static Task WaitUntilAsync(Func<bool> condition, float? timeout = null) {
			if (condition == null) throw new ArgumentNullException(nameof(condition));
			var taskCompletionSource = new TaskCompletionSource<bool>();

			Coroutiner.WaitUntil(condition,
				() => taskCompletionSource.SetResult(true),
				timeout.HasValue ?
					() => { taskCompletionSource.SetException(new TimeoutException()); } :
					null,
				timeout);

			return taskCompletionSource.Task;
		}
    }
}
