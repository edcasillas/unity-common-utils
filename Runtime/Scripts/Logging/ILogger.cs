using CommonUtils.Verbosables;
using Object = UnityEngine.Object;

namespace CommonUtils.Logging
{
    public interface ILogger
    {
		void Log(LogLevel logLevel, string message, Object context = null);
    }
}
