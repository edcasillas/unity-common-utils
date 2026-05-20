using System.Collections.Generic;
using System.Linq;

namespace CommonUtils.Editor.LaunchctlEnvironment {
	internal static class LaunchctlEnvironmentParser {
		private const string environmentBlockStart = "environment = {";

		public static List<LaunchctlEnvironmentVariable> ParseEnvironmentVariables(string launchctlPrintOutput) {
			var variables = new List<LaunchctlEnvironmentVariable>();
			if (string.IsNullOrEmpty(launchctlPrintOutput)) return variables;

			var insideEnvironmentBlock = false;
			var lines = launchctlPrintOutput.Split('\n');
			foreach (var line in lines) {
				var trimmedLine = line.Trim();
				if (!insideEnvironmentBlock) {
					insideEnvironmentBlock = trimmedLine == environmentBlockStart;
					continue;
				}

				if (trimmedLine == "}") break;

				var separatorIndex = trimmedLine.IndexOf(" => ");
				if (separatorIndex < 0) continue;

				var name = trimmedLine.Substring(0, separatorIndex);
				var value = trimmedLine.Substring(separatorIndex + " => ".Length);
				variables.Add(new LaunchctlEnvironmentVariable(name, value));
			}

			return variables.OrderBy(variable => variable.Name).ToList();
		}
	}
}
