using System;

namespace CommonUtils {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class ShowInInspectorAttribute : Attribute {
		public bool EnableSetter { get; set; }
		public bool UseTextArea { get; set; }
		public string DisplayName { get; set; }
	}
}