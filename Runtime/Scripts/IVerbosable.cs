namespace CommonUtils {
	/// <summary>
	/// Enables classes to conditionally send logs to the console.
	/// </summary>
	public interface IVerbosable {
		/// <summary>
		/// Gets a value indicating whether this object should send messages to the log console.
		/// </summary>
		bool IsVerbose { get; }
	}
}