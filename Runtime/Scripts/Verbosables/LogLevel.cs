using System;

namespace CommonUtils.Verbosables
{
    public enum LogLevel
    {
		Debug, Warning, Error
    }

	internal static class LogLevelExtensions {
		internal static Verbosity ToVerbosity(this LogLevel logLevel) {
			switch (logLevel) {
				case LogLevel.Debug:
					return Verbosity.Debug;
				case LogLevel.Warning:
					return Verbosity.Warning;
				case LogLevel.Error:
					return Verbosity.Error;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}
