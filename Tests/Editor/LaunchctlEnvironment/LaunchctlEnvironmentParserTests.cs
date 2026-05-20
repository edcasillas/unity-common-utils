using CommonUtils.Editor.LaunchctlEnvironment;
using NUnit.Framework;

namespace Tests.Editor.LaunchctlEnvironment {
	public class LaunchctlEnvironmentParserTests {
		[Test]
		public void ParseEnvironmentVariablesReadsGuiEnvironmentBlock() {
			var output = @"gui/501 = {
	type = login

	environment = {
		GAME_DEV_AUTH_TOKEN => abc123
		PATH => /usr/local/bin:/usr/bin:/bin
		SSH_AUTH_SOCK => /private/tmp/com.apple.launchd.Test/Listeners
	}

	services = {
	}
}";

			var variables = LaunchctlEnvironmentParser.ParseEnvironmentVariables(output);

			Assert.AreEqual(3, variables.Count);
			Assert.AreEqual("GAME_DEV_AUTH_TOKEN", variables[0].Name);
			Assert.AreEqual("abc123", variables[0].Value);
			Assert.AreEqual("PATH", variables[1].Name);
			Assert.AreEqual("/usr/local/bin:/usr/bin:/bin", variables[1].Value);
			Assert.AreEqual("SSH_AUTH_SOCK", variables[2].Name);
			Assert.AreEqual("/private/tmp/com.apple.launchd.Test/Listeners", variables[2].Value);
		}
	}
}
