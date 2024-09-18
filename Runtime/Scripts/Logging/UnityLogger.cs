using CommonUtils.Verbosables;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Logging {
	public class UnityLogger : ILogger {
		public void Log(LogLevel logLevel, string message, Object context = null) {
			switch (logLevel) {
				case LogLevel.Debug:
					Debug.Log(message, context);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(message, context);
					break;
				case LogLevel.Error:
					Debug.LogError(message, context);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}