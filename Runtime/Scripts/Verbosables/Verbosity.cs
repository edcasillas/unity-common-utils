using System;

namespace CommonUtils.Verbosables
{
	[Flags]
	public enum Verbosity {
		None = 0,
		Debug = 1,
		Warning = 2,
		Error = 4
	}
}
