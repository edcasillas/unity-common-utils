namespace CommonUtils.Verbosables
{
	internal static class GlobalVerbosityLevel {
		public static Verbosity? Current {
			get {
#if GLOBAL_VERBOSITY_NONE
        return Verbosity.None;
#elif GLOBAL_VERBOSITY_DEBUG
        return Verbosity.Debug | Verbosity.Warning | Verbosity.Error;
#elif GLOBAL_VERBOSITY_WARNING
        return Verbosity.Warning | Verbosity.Error;
#elif GLOBAL_VERBOSITY_ERROR
        return Verbosity.Error;
#else
				return null;
#endif
			}
		}
	}
}
