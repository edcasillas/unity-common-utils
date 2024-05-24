using System;

namespace CommonUtils.Verbosables {
	/// <summary>
	/// Enables classes to conditionally send logs to the console.
	/// </summary>
	public interface IVerbosable {
		/// <summary>
		/// Gets a value indicating the levels of verbosity for this object.
		/// </summary>
		Verbosity Verbosity { get; }

		/// <summary>
		/// Gets a value indicating whether this object should send messages to the log console.
		/// </summary>
		[Obsolete]
		bool IsVerbose { get; }
	}
}