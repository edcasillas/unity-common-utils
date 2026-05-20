namespace CommonUtils.Editor.LaunchctlEnvironment {
	internal class LaunchctlEnvironmentVariable {
		public string Name { get; }
		public string Value { get; set; }

		public LaunchctlEnvironmentVariable(string name, string value) {
			Name = name;
			Value = value;
		}
	}
}
