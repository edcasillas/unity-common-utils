namespace CommonUtils.Verbosables {
	/// <summary>
	/// Enables classes to conditionally send logs to the console.
	/// </summary>
	public interface IVerbosable {
		/// <summary>
		/// Gets a value indicating the levels of verbosity for this object.
		/// </summary>
		Verbosity Verbosity { get; }
	}
}